//============================================================
// STUDENT NAME: RENZO RIVERA CANARE
// NUS User ID.: e0543502
// COMMENTS TO GRADER:
//
//============================================================

#include <stdlib.h>
#include <stdio.h>
#include <math.h>
#include <cstdlib>
#include <ctime>

#ifdef __APPLE__
#define GL_SILENCE_DEPRECATION
#include <GLUT/glut.h>
#else
#include <GL/glut.h>
#endif

/////////////////////////////////////////////////////////////////////////////
// CONSTANTS
/////////////////////////////////////////////////////////////////////////////

#define PI                  3.1415926535897932384626433832795

#define MAX_NUM_OF_DISCS    200     // Limit the number of discs.
#define MIN_RADIUS          10.0    // Minimum radius of disc.
#define MAX_RADIUS          50.0    // Maximum radius of disc.
#define NUM_OF_SIDES        18      // Number of polygon sides to approximate a disc.

#define MIN_X_SPEED         1.0     // Minimum speed of disc in X direction.
#define MAX_X_SPEED         20.0    // Maximum speed of disc in X direction.
#define MIN_Y_SPEED         1.0     // Minimum speed of disc in Y direction.
#define MAX_Y_SPEED         20.0    // Maximum speed of disc in Y direction.

#define DESIRED_FPS         30      // Desired number of frames per second.


/////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
/////////////////////////////////////////////////////////////////////////////

typedef struct discType
{
    double pos[2];          // The X and Y coordinates of the center of the disc.
    double speed[2];        // The velocity of the disc in X and Y directions. Can be negative.
    double radius;          // Radius of the disc.
    unsigned char color[3]; // RGB color of the disc. Each value is 0 to 255.
} discType;

/*
The 2D space in which the discs are located spans from the coordinates [0, 0],
which corresponds to the bottom-leftmost corner of the display window, to the
coordinates [winWidth, winHeight], which corresponds to the top-rightmost
corner of the display window.

The speed is measured as the distance moved in the above 2D space per
render frame time.
*/

int numDiscs = 0;                   // Number of discs that have been added.

discType disc[ MAX_NUM_OF_DISCS ];  // Array for storing discs.

bool drawWireframe = false;         // Draw polygons in wireframe if true,
                                    // otherwise polygons are filled.

int winWidth = 800;                 // Window width in pixels.
int winHeight = 600;                // Window height in pixels.

GLdouble borderXLeft, borderXRight, borderYBottom, borderYTop; // Used for calculating border of view.

/////////////////////////////////////////////////////////////////////////////
// Draw the disc in its color using GL_TRIANGLE_FAN.
/////////////////////////////////////////////////////////////////////////////

void DrawDisc( const discType *d )
{
    static bool firstTime = true;
    static double unitDiscVertex[ NUM_OF_SIDES + 1 ][2]; // First part of array for vertex number, second part for x/y coordinates of vertex.

    unsigned int i;
    GLdouble doublePi = 2.0 * PI;
    
    // Get values of current disc to display.
    discType currDisc = *d;
    GLdouble discXPos = currDisc.pos[0];
    GLdouble discYPos = currDisc.pos[1];
    GLdouble discRadius = currDisc.radius;
    unsigned char discRColor = currDisc.color[0];
    unsigned char discGColor = currDisc.color[1];
    unsigned char discBColor = currDisc.color[2];

    if ( firstTime )
    {
        firstTime = false;

        // Pre-compute and store the vertices' positions of a unit-radius disc.
        for (i = 0; i <= NUM_OF_SIDES; i++) {
            unitDiscVertex[i][0] = (1 * cos( i * doublePi / (double) NUM_OF_SIDES) );
            unitDiscVertex[i][1] = (1 * sin( i * doublePi / (double) NUM_OF_SIDES) );
        }
    }

    // Draw the disc in its color as a GL_TRIANGLE_FAN.
    glBegin(GL_TRIANGLE_FAN);
        glColor3ub(discRColor, discGColor, discBColor); // Set color.
        glVertex2d(discXPos, discYPos); // Centre of triangle fan.
            for (i = 0; i <= NUM_OF_SIDES; i++) {
              glVertex2d( ((unitDiscVertex[i][0] * discRadius) + discXPos), ((unitDiscVertex[i][1] * discRadius) + discYPos) );
            }
    glEnd();
    glutPostRedisplay();
}



/////////////////////////////////////////////////////////////////////////////
// The display callback function.
/////////////////////////////////////////////////////////////////////////////

void MyDisplay( void )
{
    glClear( GL_COLOR_BUFFER_BIT );

    if ( drawWireframe )
        glPolygonMode( GL_FRONT_AND_BACK, GL_LINE );
    else
        glPolygonMode( GL_FRONT_AND_BACK, GL_FILL );

    for ( int i = 0; i < numDiscs; i++ ) DrawDisc( &disc[i] );

    glutSwapBuffers(); // Swap buffers for Double Buffer.

}



/////////////////////////////////////////////////////////////////////////////
// The mouse callback function.
// If mouse left button is pressed, a new disc is created with its center
// at the mouse cursor position. The new disc is assigned the followings:
// (1) a random radius between MIN_RADIUS and MAX_RADIUS,
// (2) a random speed in X direction in the range
//     from -MAX_X_SPEED to -MIN_X_SPEED, and from MIN_X_SPEED to MAX_X_SPEED.
// (3) a random speed in Y direction in the range
//     from -MAX_Y_SPEED to -MIN_Y_SPEED, and from MIN_Y_SPEED to MAX_Y_SPEED.
// (4) R, G, B color, each ranges from 0 to 255.
/////////////////////////////////////////////////////////////////////////////

void MyMouse( int btn, int state, int x, int y )
{
    double randXSpeed; // Random speed of disc in X direction.
    double randYSpeed; // Random speed of disc in Y direction.

    if ( btn == GLUT_LEFT_BUTTON && state == GLUT_DOWN )
    {
        if ( numDiscs >= MAX_NUM_OF_DISCS )
            printf( "Already reached maximum number of discs.\n" );
        else
        {
            disc[ numDiscs ].pos[0] = x;
            disc[ numDiscs ].pos[1] = winHeight - 1 - y;

            // PART (1): Calculate random radius between MIN_RADIUS and MAX_RADIUS.
            disc[ numDiscs ].radius = MIN_RADIUS + ( (double)(rand()) / (double)( RAND_MAX / ( MAX_RADIUS - MIN_RADIUS ) ) );

            // PART (2): Calculate random X speed between -MAX_X_SPEED and -MIN_X_SPEED, and from MIN_X_SPEED to MAX_X_SPEED.
            do {
                randXSpeed = ( (double)(rand()) / (double)( RAND_MAX / ( MAX_X_SPEED * 2 ) ) ) - MAX_X_SPEED;
            } while ( ( randXSpeed > ( -1.0 * MIN_X_SPEED ) ) && ( randXSpeed < MIN_X_SPEED ) ); // Regenerate random value if outside of defined X speed range.
            disc[ numDiscs ].speed[0] = randXSpeed;

            // PART (3): Calculate random Y speed between -MAX_Y_SPEED and -MIN_Y_SPEED, and from MIN_Y_SPEED to MAX_Y_SPEED.
            do {
                randYSpeed = ( (double)(rand()) / (double)( RAND_MAX / ( MAX_Y_SPEED * 2 ) ) ) - MAX_Y_SPEED;
            } while ( (randYSpeed > (-1.0 * MIN_Y_SPEED ) ) && ( randYSpeed < MIN_Y_SPEED ) ); // Regenerate random value if outside of defined Y speed range.
            disc[ numDiscs ].speed[1] = randYSpeed;

            // PART (4): R, G, B color, each ranges from 0 to 255.
            disc[ numDiscs ].color[0] = (unsigned char)rand() % 256;
            disc[ numDiscs ].color[1] = (unsigned char)rand() % 256;
            disc[ numDiscs ].color[2] = (unsigned char)rand() % 256;
            
            numDiscs++;
            glutPostRedisplay();
        }
    }
}



/////////////////////////////////////////////////////////////////////////////
// The reshape callback function.
// It also sets up the viewing.
/////////////////////////////////////////////////////////////////////////////

void MyReshape( int w, int h )
{
    winWidth = w;
    winHeight = h;

    glViewport( 0, 0, w, h );

    glMatrixMode( GL_PROJECTION );
    glLoadIdentity();

    // Set projection to viewport size, no scaling required.
    borderXLeft = 0 ;
    borderXRight = w;
    borderYBottom = 0;
    borderYTop = h;
    gluOrtho2D(borderXLeft, borderXRight, borderYBottom, borderYTop);

    glMatrixMode( GL_MODELVIEW );
    glLoadIdentity();
}



/////////////////////////////////////////////////////////////////////////////
// The keyboard callback function.
/////////////////////////////////////////////////////////////////////////////

void MyKeyboard( unsigned char key, int x, int y )
{
    switch ( key )
    {
        // Quit program.
        case 'q':
        case 'Q': exit(0);
                  break;

        // Toggle wireframe or filled polygons.
        case 'w':
        case 'W': drawWireframe = !drawWireframe;
                  glutPostRedisplay();
                  break;

        // Reset and erase all discs.
        case 'r':
        case 'R': numDiscs = 0;
                  glutPostRedisplay();
                  break;
    }
}



/////////////////////////////////////////////////////////////////////////////
// Updates the positions of all the discs.
//
// Increments the position of each disc by its speed in each of the
// X and Y directions. Note that the speeds can be negative.
// At its new position, if the disc is entirely or partially outside the
// left window boundary, then shift it right so that it is inside the
// window and just touches the left window boundary. Its speed in the X
// direction must now be reversed (negated). Similar approach is
// applied for the cases of the right, top, and bottom window boundaries.
/////////////////////////////////////////////////////////////////////////////

void UpdateAllDiscPos( void )
{
    GLdouble minXPos, maxXPos, minYPos, maxYPos;

    for ( int i = 0; i < numDiscs; i++ )
    {
        // Get current position & speed of disc.
        double discXPos = disc[i].pos[0];
        double discYPos = disc[i].pos[1];
        double discXSpeed = disc[i].speed[0];
        double discYSpeed = disc[i].speed[1];

        // Update location of disc based on speed (increment by speed).
        disc[i].pos[0] += discXSpeed;
        disc[i].pos[1] += discYSpeed;

        // Get min/max disc positions within boundary.
        minXPos = borderXLeft + disc[i].radius;
        maxXPos = borderXRight -  disc[i].radius;
        minYPos = borderYBottom + disc[i].radius;
        maxYPos = borderYTop -  disc[i].radius;

        // Set reverse direction for all boundaries.
        if (discXPos > maxXPos) {
            // Right boundary.
            disc[i].pos[0] = maxXPos;
            disc[i].speed[0] = -1 * disc[i].speed[0];
        }
        else if (discXPos < minXPos) {
            // Left boundary.
            disc[i].pos[0] = minXPos;
            disc[i].speed[0] = -1 * disc[i].speed[0];
        }

        if (discYPos > maxYPos) {
            // Top boundary.
            disc[i].pos[1] = maxYPos;
            disc[i].speed[1] = -1 * disc[i].speed[1];
        }
        else if (discYPos < minYPos) {
            // Bottom boundary.
            disc[i].pos[1] = minYPos;
            disc[i].speed[1] = -1 * disc[i].speed[1];
            }
    }
    glutPostRedisplay();
}



/////////////////////////////////////////////////////////////////////////////
// The timer callback function.
/////////////////////////////////////////////////////////////////////////////

void MyTimer( int v )
{
    UpdateAllDiscPos();
    glutTimerFunc(1000 / DESIRED_FPS, MyTimer, 0); // Timer function to delay framerate to desired FPS
    glutPostRedisplay();
}



/////////////////////////////////////////////////////////////////////////////
// The init function. It initializes some OpenGL states.
/////////////////////////////////////////////////////////////////////////////

void MyInit( void )
{
    glClearColor( 0.0, 0.0, 0.0, 1.0 ); // Black background color.
    glShadeModel( GL_FLAT );

    srand((unsigned)(time(NULL))); // Seed random generator.
}



static void WaitForEnterKeyBeforeExit(void)
{
    printf("Press Enter to exit.\n");
    fflush(stdin);
    getchar();
}



/////////////////////////////////////////////////////////////////////////////
// The main function.
/////////////////////////////////////////////////////////////////////////////

int main( int argc, char** argv )
{
    atexit(WaitForEnterKeyBeforeExit); // atexit() is declared in stdlib.h

    glutInit( &argc, argv );

    glutInitDisplayMode( GLUT_RGB | GLUT_DOUBLE ); // Change to Double Buffer

    glutInitWindowSize( winWidth, winHeight );
    glutCreateWindow( "main" );

    MyInit();

    // Register the callback functions.
    glutDisplayFunc( MyDisplay );
    glutReshapeFunc( MyReshape );
    glutMouseFunc( MyMouse );
    glutKeyboardFunc( MyKeyboard );

    glutTimerFunc( (1000 / DESIRED_FPS), MyTimer, 0 );
    
    // Display user instructions in console window.
    printf( "Click LEFT MOUSE BUTTON in window to add new disc.\n" );
    printf( "Press 'w' to toggle wireframe.\n" );
    printf( "Press 'r' to erase all discs.\n" );
    printf( "Press 'q' to quit.\n\n" );

    // Enter GLUT event loop.
    glutMainLoop();
    return 0;
}
