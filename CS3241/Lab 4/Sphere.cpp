//============================================================
// STUDENT NAME: RENZO RIVERA CANARE
// NUS User ID.: E0543502 
// COMMENTS TO GRADER:
//
// ============================================================

#include <cmath>
#include "Sphere.h"

using namespace std;



bool Sphere::hit( const Ray &r, double tmin, double tmax, SurfaceHitRecord &rec ) const 
{
    // Sphere not centered at origin, calculate new ray origin. 
    Vector3d newOrigin = r.origin() - center;

    // Calculate discriminant for Ray-Sphere Intersection.
    double a = 1; // a = |Rd| = 1.
    double b = 2 * dot( r.direction(), newOrigin ); // b = 2 * dot(Rd, Ro).
    double c = dot( newOrigin, newOrigin ) - ( radius * radius ); // c = dot(Ro, Ro) - r^2.
    double d = ( b * b ) - ( 4 * a * c ); // Discriminant, d = b^2 - 4ac.

    // For no intersections, d < 0.
    if ( d < 0 )
    {
        return false;
    }
    // If intersect, choose closest positive value of t.
    else
    {
        double t0;
        double tplus = ( -b + sqrt( d ) ) / ( 2 * a );
        double tminus = ( -b - sqrt( d ) ) / ( 2 * a );

        // If both less than min threshold.
        if ( tplus < tmin && tminus < tmin )
        {
            return false;
        }
        // If plus meets min threshold but minus does not.
        else if ( tplus >= tmin && tminus <= tmin )
        {
            t0 = tplus;
        }
        // If minus meets min threshold but plus does not.
        else if (tminus >= tmin && tplus <= tmin)
        {
            t0 = tminus;
        }
        // If both meet min threshold, pick smallest.
        else
        {
            t0 = min( tplus, tminus );
        }

        // If t0 is outside of max threshold.
        if (t0 > tmax)
        {
            return false;
        }

        // Calculate normal to intersection.
        rec.t = t0;
        rec.p = r.pointAtParam( t0 ); // Point of intersection.
        Vector3d calc = rec.p - center;
        rec.normal = calc / calc.length(); // Surface normal at point of intersection.
        rec.material = material; // Material of surface.

        return true;
    }
}



bool Sphere::shadowHit( const Ray &r, double tmin, double tmax ) const 
{
    // Sphere not centered at origin, calculate new ray origin. 
    Vector3d newOrigin = r.origin() - center;

    // Calculate discriminant for Ray-Sphere Intersection.
    double a = 1; // a = |Rd| = 1.
    double b = 2 * dot( r.direction(), newOrigin ); // b = 2 * dot(Rd, Ro).
    double c = dot( newOrigin, newOrigin ) - ( radius * radius ); // c = dot(Ro, Ro) - r^2.
    double d = ( b * b ) - ( 4 * a * c ); // Discriminant, d = b^2 - 4ac.

    // For no intersections, d < 0.
    if ( d < 0 )
    {
        return false;
    }
    // If intersect, choose closest positive value of t.
    else
    {
        double t0;
        double tplus = ( -b + sqrt( d ) ) / ( 2 * a );
        double tminus = ( -b - sqrt( d ) ) / ( 2 * a );

        // If both less than min threshold.
        if ( tplus < tmin && tminus < tmin )
        {
            return false;
        }
        // If plus meets min threshold but minus does not.
        else if ( tplus >= tmin && tminus <= tmin )
        {
            t0 = tplus;
        }
        // If minus meets min threshold but plus does not.
        else if ( tminus >= tmin && tplus <= tmin )
        {
            t0 = tminus;
        }
        // If both meet min threshold, pick smallest.
        else
        {
            t0 = min( tplus, tminus );
        }

        // Shadow only hit when t0 <= tmax.
        if ( t0 <= tmax )
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
