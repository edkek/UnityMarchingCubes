﻿// Perlin noise library ported from C to C#
// Original source: https://github.com/nothings/stb/blob/master/stb_perlin.h

using UnityEngine;

public static class PerlinNoise
{
    // not same permutation table as Perlin's reference to avoid copyright issues;
    // Perlin's table can be found at http://mrl.nyu.edu/~perlin/noise/
    // @OPTIMIZE: should this be unsigned char instead of int for cache?
    static int[] stb__perlin_randtab = new int[]
    {
       23, 125, 161, 52, 103, 117, 70, 37, 247, 101, 203, 169, 124, 126, 44, 123,
       152, 238, 145, 45, 171, 114, 253, 10, 192, 136, 4, 157, 249, 30, 35, 72,
       175, 63, 77, 90, 181, 16, 96, 111, 133, 104, 75, 162, 93, 56, 66, 240,
       8, 50, 84, 229, 49, 210, 173, 239, 141, 1, 87, 18, 2, 198, 143, 57,
       225, 160, 58, 217, 168, 206, 245, 204, 199, 6, 73, 60, 20, 230, 211, 233,
       94, 200, 88, 9, 74, 155, 33, 15, 219, 130, 226, 202, 83, 236, 42, 172,
       165, 218, 55, 222, 46, 107, 98, 154, 109, 67, 196, 178, 127, 158, 13, 243,
       65, 79, 166, 248, 25, 224, 115, 80, 68, 51, 184, 128, 232, 208, 151, 122,
       26, 212, 105, 43, 179, 213, 235, 148, 146, 89, 14, 195, 28, 78, 112, 76,
       250, 47, 24, 251, 140, 108, 186, 190, 228, 170, 183, 139, 39, 188, 244, 246,
       132, 48, 119, 144, 180, 138, 134, 193, 82, 182, 120, 121, 86, 220, 209, 3,
       91, 241, 149, 85, 205, 150, 113, 216, 31, 100, 41, 164, 177, 214, 153, 231,
       38, 71, 185, 174, 97, 201, 29, 95, 7, 92, 54, 254, 191, 118, 34, 221,
       131, 11, 163, 99, 234, 81, 227, 147, 156, 176, 17, 142, 69, 12, 110, 62,
       27, 255, 0, 194, 59, 116, 242, 252, 19, 21, 187, 53, 207, 129, 64, 135,
       61, 40, 167, 237, 102, 223, 106, 159, 197, 189, 215, 137, 36, 32, 22, 5,  

       // and a second copy so we don't need an extra mask or static initializer
       23, 125, 161, 52, 103, 117, 70, 37, 247, 101, 203, 169, 124, 126, 44, 123,
       152, 238, 145, 45, 171, 114, 253, 10, 192, 136, 4, 157, 249, 30, 35, 72,
       175, 63, 77, 90, 181, 16, 96, 111, 133, 104, 75, 162, 93, 56, 66, 240,
       8, 50, 84, 229, 49, 210, 173, 239, 141, 1, 87, 18, 2, 198, 143, 57,
       225, 160, 58, 217, 168, 206, 245, 204, 199, 6, 73, 60, 20, 230, 211, 233,
       94, 200, 88, 9, 74, 155, 33, 15, 219, 130, 226, 202, 83, 236, 42, 172,
       165, 218, 55, 222, 46, 107, 98, 154, 109, 67, 196, 178, 127, 158, 13, 243,
       65, 79, 166, 248, 25, 224, 115, 80, 68, 51, 184, 128, 232, 208, 151, 122,
       26, 212, 105, 43, 179, 213, 235, 148, 146, 89, 14, 195, 28, 78, 112, 76,
       250, 47, 24, 251, 140, 108, 186, 190, 228, 170, 183, 139, 39, 188, 244, 246,
       132, 48, 119, 144, 180, 138, 134, 193, 82, 182, 120, 121, 86, 220, 209, 3,
       91, 241, 149, 85, 205, 150, 113, 216, 31, 100, 41, 164, 177, 214, 153, 231,
       38, 71, 185, 174, 97, 201, 29, 95, 7, 92, 54, 254, 191, 118, 34, 221,
       131, 11, 163, 99, 234, 81, 227, 147, 156, 176, 17, 142, 69, 12, 110, 62,
       27, 255, 0, 194, 59, 116, 242, 252, 19, 21, 187, 53, 207, 129, 64, 135,
       61, 40, 167, 237, 102, 223, 106, 159, 197, 189, 215, 137, 36, 32, 22, 5,
    };

    static float stb__perlin_lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    static float[] basis = new float[]
    {
         1, 1, 0 ,
        -1, 1, 0 ,
         1,-1, 0 ,
        -1,-1, 0 ,
         1, 0, 1 ,
        -1, 0, 1 ,
         1, 0,-1 ,
        -1, 0,-1 ,
         0, 1, 1 ,
         0,-1, 1 ,
         0, 1,-1 ,
         0,-1,-1 ,
    };

    static byte[] indices = new byte[]
    {
        0,1,2,3,4,5,6,7,8,9,10,11,
        0,9,1,11,
        0,1,2,3,4,5,6,7,8,9,10,11,
        0,1,2,3,4,5,6,7,8,9,10,11,
        0,1,2,3,4,5,6,7,8,9,10,11,
        0,1,2,3,4,5,6,7,8,9,10,11,
    };

    // different grad function from Perlin's, but easy to modify to match reference
    static float stb__perlin_grad(int hash, float x, float y, float z)
    {
        // perlin's gradient has 12 cases so some get used 1/16th of the time
        // and some 2/16ths. We reduce bias by changing those fractions
        // to 5/16ths and 6/16ths, and the same 4 cases get the extra weight.
        // if you use reference permutation table, change 63 below to 15 to match reference
        int gradIndex = indices[hash & 63];
        return basis[gradIndex]*x + basis[gradIndex+1]*y + basis[gradIndex+2]*z;
    }

    public static float PerlinNoise3(float x, float y, float z, int x_wrap = 0, int y_wrap = 0, int z_wrap = 0)
    {
        float u, v, w;
        float n000, n001, n010, n011, n100, n101, n110, n111;
        float n00, n01, n10, n11;
        float n0, n1;

        int x_mask = (x_wrap - 1) & 255;
        int y_mask = (y_wrap - 1) & 255;
        int z_mask = (z_wrap - 1) & 255;
        int px = (int)Mathf.Floor(x);
        int py = (int)Mathf.Floor(y);
        int pz = (int)Mathf.Floor(z);
        int x0 = px & x_mask, x1 = (px + 1) & x_mask;
        int y0 = py & y_mask, y1 = (py + 1) & y_mask;
        int z0 = pz & z_mask, z1 = (pz + 1) & z_mask;
        int r0, r1, r00, r01, r10, r11;

        x -= px; u = (((x * 6 - 15) * x + 10) * x * x * x);
        y -= py; v = (((y * 6 - 15) * y + 10) * y * y * y);
        z -= pz; w = (((z * 6 - 15) * z + 10) * z * z * z);

        r0 = stb__perlin_randtab[x0];
        r1 = stb__perlin_randtab[x1];

        r00 = stb__perlin_randtab[r0 + y0];
        r01 = stb__perlin_randtab[r0 + y1];
        r10 = stb__perlin_randtab[r1 + y0];
        r11 = stb__perlin_randtab[r1 + y1];

        n000 = stb__perlin_grad(stb__perlin_randtab[r00 + z0], x, y, z);
        n001 = stb__perlin_grad(stb__perlin_randtab[r00 + z1], x, y, z - 1);
        n010 = stb__perlin_grad(stb__perlin_randtab[r01 + z0], x, y - 1, z);
        n011 = stb__perlin_grad(stb__perlin_randtab[r01 + z1], x, y - 1, z - 1);
        n100 = stb__perlin_grad(stb__perlin_randtab[r10 + z0], x - 1, y, z);
        n101 = stb__perlin_grad(stb__perlin_randtab[r10 + z1], x - 1, y, z - 1);
        n110 = stb__perlin_grad(stb__perlin_randtab[r11 + z0], x - 1, y - 1, z);
        n111 = stb__perlin_grad(stb__perlin_randtab[r11 + z1], x - 1, y - 1, z - 1);

        n00 = stb__perlin_lerp(n000, n001, w);
        n01 = stb__perlin_lerp(n010, n011, w);
        n10 = stb__perlin_lerp(n100, n101, w);
        n11 = stb__perlin_lerp(n110, n111, w);

        n0 = stb__perlin_lerp(n00, n01, v);
        n1 = stb__perlin_lerp(n10, n11, v);

        return stb__perlin_lerp(n0, n1, u);
    }
}