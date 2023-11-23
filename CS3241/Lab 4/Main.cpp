//============================================================
// STUDENT NAME: RENZO RIVERA CANARE
// NUS User ID.: E0543502   
// COMMENTS TO GRADER: Tried to make a bomb whose ignition
// flame you could see in the reflection!
// ============================================================

#include "Util.h"
#include "Vector3d.h"
#include "Color.h"
#include "Image.h"
#include "Ray.h"
#include "Camera.h"
#include "Material.h"
#include "Light.h"
#include "Surface.h"
#include "Sphere.h"
#include "Plane.h"
#include "Triangle.h"
#include "Scene.h"
#include "Raytrace.h"
#include <string>


// Constants for Scene 1.
static constexpr int imageWidth1 = 640;
static constexpr int imageHeight1 = 480;
static constexpr int reflectLevels1 = 2;  // 0 -- object does not reflect scene.
static constexpr int hasShadow1 = true;
static constexpr std::string_view outImageFile1 = "out1.png";

// Constants for Scene 2.
static constexpr int imageWidth2 = 640;
static constexpr int imageHeight2 = 480;
static constexpr int reflectLevels2 = 2;  // 0 -- object does not reflect scene.
static constexpr int hasShadow2 = true;
static constexpr std::string_view outImageFile2 = "out2.png";



///////////////////////////////////////////////////////////////////////////
// Raytrace the whole image of the scene and write it to a file.
///////////////////////////////////////////////////////////////////////////

void RenderImage( const std::string &imageFilename, const Scene &scene, 
                  int reflectLevels, bool hasShadow )
{
    int imgWidth = scene.camera.getImageWidth();
    int imgHeight = scene.camera.getImageHeight();

    Image image( imgWidth, imgHeight ); // To store the result of ray tracing.

    double startTime = Util::GetCurrRealTime();
    double startCPUTime = Util::GetCurrCPUTime();

    // Generate image, rendering in parallel on Windows and Linux.
    #ifndef __APPLE__
    #pragma warning( push )
    #pragma warning( disable : 6993 )
    #pragma omp parallel for
    #endif
    for ( int y = 0; y < imgHeight; y++ )
    {
        double pixelPosY = y + 0.5;

        for ( int x = 0; x < imgWidth; x++ )
        {
            double pixelPosX = x + 0.5;
            Ray ray = scene.camera.getRay( pixelPosX, pixelPosY );
            Color pixelColor = Raytrace::TraceRay( ray, scene, reflectLevels, hasShadow );
            pixelColor.clamp();
            image.setPixel( x, y, pixelColor );
        }
    }
    #ifndef __APPLE__
    #pragma warning( pop )
    #endif

    double cpuTimeElapsed = Util::GetCurrCPUTime() - startCPUTime;
    double realTimeElapsed = Util::GetCurrRealTime() - startTime;
    std::cout << "CPU time taken = " << cpuTimeElapsed << "sec" << std::endl;
    std::cout << "Real time taken = " << realTimeElapsed << "sec" << std::endl;

    // Write image to file.
    if ( !image.writeToFile( imageFilename ) ) return;
    else Util::ErrorExit("File: %s could not be written.\n", imageFilename.c_str() );
}



// Forward declarations. These functions are defined later in the file.
void DefineScene1( Scene &scene, int imageWidth, int imageHeight );
void DefineScene2( Scene &scene, int imageWidth, int imageHeight );



int main()
{
// Define Scene 1.

    Scene scene1;
    DefineScene1( scene1, imageWidth1, imageHeight1 );

// Render Scene 1.

    std::cout << "Render Scene 1..." << std::endl;
    RenderImage( std::string(outImageFile1), scene1, reflectLevels1, hasShadow1 );
    std::cout << "Scene 1 completed." << std::endl;

// Delete Scene 1 surfaces.

    for (auto& surface : scene1.surfaces)
    {
        delete surface;
    }


// Define Scene 2.

    Scene scene2;
    DefineScene2( scene2, imageWidth2, imageHeight2 );

// Render Scene 2.

    std::cout << "Render Scene 2..." << std::endl;
    RenderImage( std::string(outImageFile2), scene2, reflectLevels2, hasShadow2 );
    std::cout << "Scene 2 completed." << std::endl;

// Delete Scene 2 surfaces.

    for (auto& surface : scene2.surfaces)
    {
        delete surface;
    }


    std::cout << "All done. Press Enter to exit." << std::endl;
    std::cin.get();
    return 0;
}



///////////////////////////////////////////////////////////////////////////
// Modelling of Scene 1.
///////////////////////////////////////////////////////////////////////////

void DefineScene1( Scene &scene, int imageWidth, int imageHeight )
{
    scene.backgroundColor = Color( 0.2f, 0.3f, 0.5f );

    scene.amLight.I_a = Color( 1.0f, 1.0f, 1.0f ) * 0.25f;

// Define materials.

    // Light red.
    Material lightRed = Material();
    lightRed.k_d = Color( 0.8f, 0.4f, 0.4f );
    lightRed.k_a = lightRed.k_d;
    lightRed.k_r = Color( 0.8f, 0.8f, 0.8f ) / 1.5f;
    lightRed.k_rg = Color( 0.8f, 0.8f, 0.8f ) / 3.0f;
    lightRed.n = 64.0f;

    // Light green.
    Material lightGreen = Material();
    lightGreen.k_d = Color( 0.4f, 0.8f, 0.4f );
    lightGreen.k_a = lightGreen.k_d;
    lightGreen.k_r = Color( 0.8f, 0.8f, 0.8f ) / 1.5f;
    lightGreen.k_rg = Color( 0.8f, 0.8f, 0.8f ) / 3.0f;
    lightGreen.n = 64.0f;

    // Light blue.
    Material lightBlue = Material();
    lightBlue.k_d = Color( 0.4f, 0.4f, 0.8f ) * 0.9f;
    lightBlue.k_a = lightBlue.k_d;
    lightBlue.k_r = Color( 0.8f, 0.8f, 0.8f ) / 1.5f;
    lightBlue.k_rg = Color( 0.8f, 0.8f, 0.8f ) / 2.5f;
    lightBlue.n = 64.0f;

    // Yellow.
    Material yellow = Material();
    yellow.k_d = Color( 0.6f, 0.6f, 0.2f );
    yellow.k_a = yellow.k_d;
    yellow.k_r = Color( 0.8f, 0.8f, 0.8f ) / 1.5f;
    yellow.k_rg = Color( 0.8f, 0.8f, 0.8f ) / 3.0f;
    yellow.n = 64.0f;

    // Gray.
    Material gray = Material();
    gray.k_d = Color( 0.6f, 0.6f, 0.6f );
    gray.k_a = gray.k_d;
    gray.k_r = Color( 0.6f, 0.6f, 0.6f );
    gray.k_rg = Color( 0.8f, 0.8f, 0.8f ) / 3.0f;
    gray.n = 128.0f;

    // Insert into scene materials vector.
    scene.materials = { lightRed, lightGreen, lightBlue, yellow, gray };


// Define point light sources.

    scene.ptLights.resize(2);

    PointLightSource light0 = { Vector3d(100.0, 120.0, 10.0), Color(1.0f, 1.0f, 1.0f) * 0.6f };
    PointLightSource light1 = { Vector3d(5.0, 80.0, 60.0), Color(1.0f, 1.0f, 1.0f) * 0.6f };

    scene.ptLights = { light0, light1 };


// Define surface primitives.

    scene.surfaces.resize(15);

    auto horzPlane = new Plane( 0.0, 1.0, 0.0, 0.0, scene.materials[2] ); // Horizontal plane.
    auto leftVertPlane = new Plane( 1.0, 0.0, 0.0, 0.0, scene.materials[4] ); // Left vertical plane.
    auto rightVertPlane = new Plane( 0.0, 0.0, 1.0, 0.0, scene.materials[4] ); // Right vertical plane.
    auto bigSphere =  new Sphere( Vector3d( 40.0, 20.0, 42.0 ), 22.0, scene.materials[0] ); // Big sphere.
    auto smallSphere = new Sphere( Vector3d( 75.0, 10.0, 40.0 ), 12.0, scene.materials[1] ); // Small sphere.

    // Cube +y face.
    auto cubePosYTri1 = new Triangle( Vector3d( 50.0, 20.0, 90.0 ),
                                      Vector3d( 50.0, 20.0, 70.0 ),
                                      Vector3d( 30.0, 20.0, 70.0 ),
                                      scene.materials[3] );
    auto cubePosYTri2 = new Triangle( Vector3d( 50.0, 20.0, 90.0 ),
                                      Vector3d( 30.0, 20.0, 70.0 ),
                                      Vector3d( 30.0, 20.0, 90.0 ),
                                      scene.materials[3] );

    // Cube +x face.
    auto cubePosXTri1 = new Triangle( Vector3d( 50.0, 0.0, 70.0 ),
                                      Vector3d( 50.0, 20.0, 70.0 ),
                                      Vector3d( 50.0, 20.0, 90.0 ),
                                      scene.materials[3]);
    auto cubePosXTri2 = new Triangle( Vector3d( 50.0, 0.0, 70.0 ),
                                      Vector3d( 50.0, 20.0, 90.0 ),
                                      Vector3d( 50.0, 0.0, 90.0 ),
                                      scene.materials[3] );

    // Cube -x face.
    auto cubeNegXTri1 = new Triangle( Vector3d( 30.0, 0.0, 90.0 ),
                                      Vector3d( 30.0, 20.0, 90.0 ),
                                      Vector3d( 30.0, 20.0, 70.0 ),
                                      scene.materials[3]);
    auto cubeNegXTri2 = new Triangle( Vector3d( 30.0, 0.0, 90.0 ),
                                      Vector3d( 30.0, 20.0, 70.0 ),
                                      Vector3d( 30.0, 0.0, 70.0 ),
                                      scene.materials[3] );

    // Cube +z face.
    auto cubePosZTri1 = new Triangle( Vector3d( 50.0, 0.0, 90.0 ),
                                      Vector3d( 50.0, 20.0, 90.0 ),
                                      Vector3d( 30.0, 20.0, 90.0 ),
                                      scene.materials[3]);
    auto cubePosZTri2 = new Triangle( Vector3d( 50.0, 0.0, 90.0 ),
                                      Vector3d( 30.0, 20.0, 90.0 ),
                                      Vector3d( 30.0, 0.0, 90.0 ),
                                      scene.materials[3] );

    // Cube -z face.
    auto cubeNegZTri1 = new Triangle( Vector3d( 30.0, 0.0, 70.0 ),
                                      Vector3d( 30.0, 20.0, 70.0 ),
                                      Vector3d( 50.0, 20.0, 70.0 ),
                                      scene.materials[3] );
    auto cubeNegZTri2 = new Triangle( Vector3d( 30.0, 0.0, 70.0 ),
                                      Vector3d( 50.0, 20.0, 70.0 ),
                                      Vector3d( 50.0, 0.0, 70.0 ),
                                      scene.materials[3] );

    scene.surfaces = { horzPlane, leftVertPlane, rightVertPlane, 
                       bigSphere, smallSphere,
                       cubePosXTri1, cubePosXTri2,
                       cubePosYTri1, cubePosYTri2,
                       cubePosZTri1, cubePosZTri2,
                       cubeNegXTri1, cubeNegXTri2,
                       cubeNegZTri1, cubeNegZTri2 };


// Define camera.

    scene.camera = Camera( Vector3d( 150.0, 120.0, 150.0 ),  // eye
                           Vector3d( 45.0, 22.0, 55.0 ),  // lookAt
                           Vector3d( 0.0, 1.0, 0.0 ),  //upVector
                           (-1.0 * imageWidth) / imageHeight,  // left
                           (1.0 * imageWidth) / imageHeight,  // right
                           -1.0, 1.0, 3.0,  // bottom, top, near
                           imageWidth, imageHeight );  // image_width, image_height
}



///////////////////////////////////////////////////////////////////////////
// Modeling of Scene 2.
///////////////////////////////////////////////////////////////////////////

void DefineScene2( Scene &scene, int imageWidth, int imageHeight )
{
    scene.backgroundColor = Color( 0.2f, 0.3f, 0.5f );

    scene.amLight.I_a = Color( 1.0f, 1.0f, 1.0f ) * 0.15f;

// Define materials.

    // Light red. 0
    Material lightRed = Material();
    lightRed.k_d = Color( 1.0f, 0.2f, 0.0f );
    lightRed.k_a = lightRed.k_d;
    lightRed.k_r = Color( 0.8f, 0.8f, 0.8f );
    lightRed.k_rg = Color( 0.8f, 0.8f, 0.8f );
    lightRed.n = 128.0f;

    // Light orange. 1
    Material lightOrange = Material();
    lightOrange.k_d = Color( 1.0f, 0.8f, 0.0f );
    lightOrange.k_a = lightOrange.k_d;
    lightOrange.k_r = Color( 1.0f, 1.0f, 1.0f );
    lightOrange.k_rg = Color( 1.0f, 1.0f, 1.0f );
    lightOrange.n = 128.0f;

    // Gray. 2
    Material gray = Material();
    gray.k_d = Color( 0.6f, 0.6f, 0.6f );
    gray.k_a = gray.k_d;
    gray.k_r = Color( 0.6f, 0.6f, 0.6f );
    gray.k_rg = Color( 0.8f, 0.8f, 0.8f ) / 3.0f;
    gray.n = 128.0f;

    // Dark gray. 3
    Material darkGray = Material();
    darkGray.k_d = Color( 0.6f, 0.6f, 0.6f ) / 2.0f;
    darkGray.k_a = darkGray.k_d;
    darkGray.k_r = Color( 0.6f, 0.6f, 0.6f ) / 2.0f;
    darkGray.k_rg = Color( 0.8f, 0.8f, 0.8f ) / 6.0f;
    darkGray.n = 64.0f;

    // Brown. 4 
    Material brown = Material();
    brown.k_d = Color( 0.3f, 0.15f, 0.01f );
    brown.k_a = brown.k_d;
    brown.k_r = Color( 0.8f, 0.8f, 0.8f ) / 1.5f;
    brown.k_rg = Color( 0.8f, 0.8f, 0.8f ) / 3.0f;
    brown.n = 64.0f;

    // Black. 5
    Material black = Material();
    black.k_d = Color( 0.1f, 0.1f, 0.1f );
    black.k_a = black.k_d;
    black.k_r = Color( 0.8f, 0.8f, 0.8f ) / 1.5f;
    black.k_rg = Color( 0.8f, 0.8f, 0.8f ) / 3.0f;
    black.n = 32.0f;

    // White. 6
    Material white = Material();
    white.k_d = Color( 1.0f, 1.0f, 1.0f );
    white.k_a = white.k_d;
    white.k_r = Color( 1.0f, 1.0f, 1.0f );
    white.k_rg = Color( 1.0f, 1.0f, 1.0f );
    white.n = 32.0f;


    // Insert into scene materials vector.
    scene.materials = { lightRed, lightOrange, gray, darkGray, brown, black, white };


// Define point light sources.

    scene.ptLights.resize(3);

    PointLightSource light0 = { Vector3d( 100.0, 120.0, 50.0 ), Color( 1.0f, 1.0f, 1.0f ) * 0.6f };
    PointLightSource light1 = { Vector3d( 41.0, 47.0, 40.0 ), Color( 1.0f, 0.2f, 0.0f ) * 0.9f }; // Light of bomb.
    PointLightSource light2 = { Vector3d( 41.0, 47.0, 40.0 ), Color( 1.0f, 1.0f, 1.0f ) * 0.2f }; // Secondary lighting to exaggerate flame.

    scene.ptLights = { light0, light1, light2 };


// Define surface primitives.

    scene.surfaces.resize(28); // 2 + 6 for bomb + (6 * 3) for tiles = 28.

    auto leftVertPlane = new Plane( 1.0, 0.0, 0.0, 0.0, scene.materials[2] ); // Left vertical plane.
    auto rightVertPlane = new Plane( 0.0, 0.0, 1.0, 0.0, scene.materials[2] ); // Right vertical plane.

    auto bombSphere = new Sphere( Vector3d(38.0, 18.0, 40.0), 18.0, scene.materials[3] ); // Bomb body.
    auto bombSphereTop = new Sphere( Vector3d(38.0, 36.0, 40.0), 4.0, scene.materials[2] ); // Bomb top.
    auto bombFuse1 = new Sphere( Vector3d(38.0, 40.0, 40.0), 1.0, scene.materials[4] ); // Fuse.
    auto bombFuse2 = new Sphere( Vector3d(38.0, 41.0, 40.0), 1.0, scene.materials[4] ); // Fuse.
    auto bombFuse3 = new Sphere( Vector3d(38.0, 42.0, 40.0), 1.0, scene.materials[4] ); // Fuse.
    auto bombFuse4 = new Sphere( Vector3d(39.0, 43.0, 40.0), 1.0, scene.materials[4] ); // Fuse.
    auto bombFuse5 = new Sphere( Vector3d(40.0, 44.0, 40.0), 1.0, scene.materials[1] ); // Fuse.
    auto bombFuse6 = new Sphere( Vector3d(40.0, 45.0, 40.0), 1.0, scene.materials[0] ); // Fuse.

     // Tile Set 1 (6 tiles per set)
    auto tri1 = new Triangle( Vector3d( 0.0, 1.0, 0.0 ),               // Black.
                              Vector3d( 0.0, 1.0, 50.0 ),
                              Vector3d( 50.0, 1.0, 50.0 ),
                              scene.materials[5] );
    auto tri2 = new Triangle( Vector3d( 0.0, 1.0, 0.0 ),               // White.
                              Vector3d( 50.0, 1.0, 0.0 ),
                              Vector3d( 50.0, 1.0, 50.0 ),
                              scene.materials[6] );
    auto tri3 = new Triangle( Vector3d( 50.0, 1.0, 50.0 ),             // Black.
                              Vector3d( 50.0, 1.0, 0.0 ),
                              Vector3d( 100.0, 1.0, 50.0 ),
                              scene.materials[5] );
    auto tri4 = new Triangle( Vector3d(100.0, 1.0, 50.0 ),             // White.
                              Vector3d( 50.0, 1.0, 0.0 ),
                              Vector3d( 100.0, 1.0, 0.0 ),
                              scene.materials[6] );
    auto tri5 = new Triangle( Vector3d(100.0, 1.0, 50.0 ),             // Black.
                              Vector3d( 100.0, 1.0, 0.0 ),
                              Vector3d( 150.0, 1.0, 50.0 ),
                              scene.materials[5] );
    auto tri6 = new Triangle( Vector3d(150.0, 1.0, 50.0 ),             // White.
                              Vector3d( 100.0, 1.0, 0.0 ),
                              Vector3d( 150.0, 1.0, 0.0 ),
                              scene.materials[6] );

    // Tile Set 2 (6 tiles per set)
    auto tri7 = new Triangle( Vector3d( 0.0, 1.0, 50.0 ),              // Black.
                              Vector3d( 0.0, 1.0, 100.0 ),
                              Vector3d( 50.0, 1.0, 100.0 ),
                              scene.materials[5] );
    auto tri8 = new Triangle( Vector3d( 0.0, 1.0, 50.0 ),              // White.
                              Vector3d( 50.0, 1.0, 50.0 ),
                              Vector3d( 50.0, 1.0, 100.0 ),
                              scene.materials[6] );
    auto tri9 = new Triangle( Vector3d( 50.0, 1.0, 100.0 ),            // Black.
                              Vector3d( 50.0, 1.0, 50.0 ),
                              Vector3d( 100.0, 1.0, 100.0) ,
                              scene.materials[5] );
    auto tri10 = new Triangle( Vector3d( 100.0, 1.0, 100.0 ),          // White.
                               Vector3d( 50.0, 1.0, 50.0 ),
                               Vector3d( 100.0, 1.0, 50.0 ),
                               scene.materials[6] );
    auto tri11 = new Triangle( Vector3d( 100.0, 1.0, 100.0 ),          // Black.
                               Vector3d( 100.0, 1.0, 50.0 ),
                               Vector3d( 150.0, 1.0, 100.0 ),
                               scene.materials[5] );
    auto tri12 = new Triangle( Vector3d( 150.0, 1.0, 100.0 ),          // White.
                               Vector3d( 100.0, 1.0, 50.0 ),
                               Vector3d( 150.0, 1.0, 50.0 ),
                               scene.materials[6] );

    // Tile Set 3 (6 tiles per set)
    auto tri13 = new Triangle( Vector3d( 0.0, 1.0, 100.0 ),            // Black.
                               Vector3d( 0.0, 1.0, 150.0 ),
                               Vector3d( 50.0, 1.0, 150.0 ),
                               scene.materials[5] );
    auto tri14 = new Triangle( Vector3d( 0.0, 1.0, 100.0 ),            // White.
                               Vector3d( 50.0, 1.0, 100.0 ),
                               Vector3d( 50.0, 1.0, 150.0 ),
                               scene.materials[6] );
    auto tri15 = new Triangle( Vector3d( 50.0, 1.0, 150.0 ),           // Black.
                               Vector3d( 50.0, 1.0, 100.0 ),
                               Vector3d( 100.0, 1.0, 150.0 ),
                               scene.materials[5] );
    auto tri16 = new Triangle( Vector3d( 100.0, 1.0, 150.0 ),          // White.
                               Vector3d( 50.0, 1.0, 100.0 ),
                               Vector3d( 100.0, 1.0, 100.0 ),
                               scene.materials[6] );
    auto tri17 = new Triangle( Vector3d( 100.0, 1.0, 150.0 ),          // Black.
                               Vector3d( 100.0, 1.0, 100.0 ),
                               Vector3d( 150.0, 1.0, 150.0 ),
                               scene.materials[5]);
    auto tri18 = new Triangle( Vector3d( 150.0, 1.0, 150.0 ),          // White.
                               Vector3d( 100.0, 1.0, 100.0 ),
                               Vector3d( 150.0, 1.0, 100.0 ),
                               scene.materials[6] );


    scene.surfaces = { leftVertPlane, rightVertPlane,
                       bombSphere, bombSphereTop, bombFuse1,
                       bombFuse2, bombFuse3, bombFuse4,
                       bombFuse5, bombFuse6, tri1, tri2,
                       tri3, tri4, tri5, tri6, tri7, tri8,
                       tri9, tri10, tri11, tri12, tri13, tri14,
                       tri15, tri16, tri17, tri18 };


// Define camera.

    scene.camera = Camera( Vector3d( 150.0, 120.0, 150.0 ),  // eye
                           Vector3d( 45.0, 22.0, 55.0 ),  // lookAt
                           Vector3d( 0.0, 1.0, 0.0 ),  //upVector
                           ( -1.0 * imageWidth ) / imageHeight,  // left
                           ( 1.0 * imageWidth ) / imageHeight,  // right
                           -1.0, 1.0, 3.0,  // bottom, top, near
                           imageWidth, imageHeight );  // image_width, image_height

}
