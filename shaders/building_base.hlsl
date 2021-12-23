float PointInOrOn( float3 P1, float3 P2, float3 A, float3 B )
{
    float3 CP1 = cross(B - A, P1 - A);
    float3 CP2 = cross(B - A, P2 - A);
    return step(0.0, dot(CP1, CP2));
}

bool PointInTriangle( float3 px, float3 p0, float3 p1, float3 p2 )
{
    return 
        PointInOrOn(px, p0, p1, p2) *
        PointInOrOn(px, p1, p2, p0) *
        PointInOrOn(px, p2, p0, p1);
}

bool PointInQuad(float3 px, float3 p0, float3 p1, float3 p2, float3 p3)
{
    return
        PointInTriangle(px, p0, p1, p2) +
        PointInTriangle(px, p0, p2, p3);
}

float3 intersectPlane(float3 start, float3 dir, float3 p0, float3 p1, float3 p2)
{
    float3 D = dir;
    float3 N = cross(p1-p0, p2-p0);
    float3 X = start + D * dot(p0 - start, N) / dot(D, N);

    return X;
}

bool intersectTriangle(float3 start, float3 dir, float3 p0, float3 p1, float3 p2)
{
    float3 X = intersectPlane(start, dir, p0, p1, p2);
    return PointInTriangle(X, p0, p1, p2);
}

bool intersectQuad(float3 start, float3 dir, float3 p0, float3 p1, float3 p2, float3 p3, out float3 hit)
{
    float3 X = intersectPlane(start, dir, p0, p1, p2);
    hit = X;
    return PointInQuad(X, p0, p1, p2, p3);
}

void TransformPlane(float2 vUV, inout float3 plane[4])
{
    for(int i = 0; i < 4; i++)
    {
        plane[i].xy -= vUV;
    }
}

float3 GetFaceNormal(int index)
{
    if(index == 0) return float3(1.0, 0.0, 0.0);
    if(index == 1) return float3(-1.0, 0.0, 0.0);
    if(index == 2) return float3(0.0, 1.0, 0.0);
    if(index == 3) return float3(0.0, -1.0, 0.0);
    return float3(0.0, 0.0, 1.0);
}

bool FaceTest(float3 frag_dir, float2 vUV, float3 plane[4], out float3 hit)
{
    bool intersect = intersectQuad(float3(0.0, 0.0, 0.0), frag_dir, plane[0], plane[1], plane[2], plane[3], hit);

    hit.xy += vUV;
    hit.z *= -1;

    return intersect;
}

int CubeTest(float3 frag_dir, float2 vUV, float3 left[4], float3 right[4], float3 top[4], float3 bottom[4], float3 back[4], out float3 hit)
{
    float3 hits[5];
    hits[0] = intersectPlane(float3(0.0, 0.0, 0.0), frag_dir, left[0], left[1], left[2]);
    hits[1] = intersectPlane(float3(0.0, 0.0, 0.0), frag_dir, right[0], right[1], right[2]);
    hits[2] = intersectPlane(float3(0.0, 0.0, 0.0), frag_dir, top[0], top[1], top[2]);
    hits[3] = intersectPlane(float3(0.0, 0.0, 0.0), frag_dir, bottom[0], bottom[1], bottom[2]);
    hits[4] = intersectPlane(float3(0.0, 0.0, 0.0), frag_dir, back[0], back[1], back[2]);

    int closestFace = 4;
    float closestDist = length(hits[4]);
    for(int i = 0; i < 4; i++)
    {
        float angle = dot(frag_dir, GetFaceNormal(i));
        if(angle > 0.0)
            continue;

        float dist = length(hits[i]);
        if(dist < closestDist)
        {
            closestDist = dist;
            closestFace = i;
        }
    }

    hit = hits[closestFace];

    hit.xy += vUV;
    hit.z *= -1;

    return closestFace;
}