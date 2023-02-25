// See https://aka.ms/new-console-template for more information
/*
How it (will) work:

1. Create line equation from start point to end point ("journey base")
2. For each polygon, create lines from the center to the vertices ("center lines")
3. Check which center lines the journey base intersects
4. Split the journey base into multiple possible lines going to each vertices' center lines it intersects, to the next
    one, and to the endpoint (Note: make sure these lines don't intersect any center lines, or the machine might try 
    to go through the obstacle)
5. Go through all possible combinations of them to see which has the shortest distance
6. Make the last line of them the new journey base
7. Repeat for all obstacles

(Steps and Corresponding Numbers in attatched photo)
*/

using System;
//NOTE: OTHER CLASSES AND STUFF ALREADY ACCESSED FROM THE GEOMETRYELEMENTS FILE - NO NEED TO IMPORT/SAY "using ____;"

public class Program {
    public List<Path> paths;
    public static void Main(string[] args) {
        Polygon[] presetPolygons = new Polygon[] {};
    }
    //separate method so the entire code doesn't have to be re - executed every time
    public List<Path> FindPath(Polygon[] p, Path initialPath) {
        List<Path> paths = new List<Path>();
        //code to add paths to a list called "paths"

        //find all polygons that are in the journey's range.
        //

        //below is an <<<EXAMPLE>>> to prevent unassigned variable errors
        paths.Add(new Path(new double[] {0, 0}, new double[] {0, 0}));
        return paths;
    }
    //Call this from the outside
    public double[,] GetPaths() {
        //decode the list of paths into points
        //return statement just to get rid of error for now
        //access the paths variable of the Program class
        return new double[,] {{}};
    }
    //find if 2 lines are intersecting
    public bool LinesIntersecting (Line l1, Line l2) {
        //if they are parallel
        if (l1.slope == l2.slope && l1.intercept != l2.intercept) {
            return false;
        }
        //if their ranges don't overlap
        if (l1.xrange[0] > l2.xrange[1] || l1.xrange[1] < l2.xrange[0]) {
            return false;
        }
        //x intercept = (b2 - b1)/(m1 - m2)
        double xIntercept = (l2.intercept - l1.intercept)/(l1.slope - l2.slope);
        //figure out the range in which both lines coexist
        double interceptRangeBottom;
        double interceptRangeTop;

        //if the first line's left limit is more than the second one's, then make that the left limit of the valid
        //interception range
        if (l1.xrange[0] > l2.xrange[0]) {
            interceptRangeBottom = l1.xrange[0];
        } else {
            interceptRangeBottom = l2.xrange[0];
        }
        //same with right, but which is less
        if (l1.xrange[1] < l2.xrange[1]) {
            interceptRangeTop = l1.xrange[1];
        } else {
            interceptRangeTop = l2.xrange[1];
        }

        //check if the xIntercept is within this range
        if (xIntercept >= interceptRangeBottom && xIntercept <= interceptRangeTop) {
            return true;
        } else {
            return false;
        }
    }
}
public class Path {
    public Path (double[] sP, double[] eP) {
        StartPoint = sP;
        EndPoint = eP;
    }
    private double[] startPoint;
    public double[] StartPoint {
        get {return startPoint;}
        //Only set startpoint if value can be coordinates. Otherwise, set the x and y to the first and second values
        //Same with endPoint
        set {
            if (value.Length == 2) {
                startPoint = value;
            } else {
                startPoint[0] = value[0]; 
                startPoint[1] = value[1];}
            }
    }
    private double[] endPoint;
    public double[] EndPoint {
        get {return endPoint;}
        //Only set endpoint if value can be coordinates. Otherwise, set the x and y to the first and second values
        set {
            if (value.Length == 2) {
                endPoint = value;
            } else {
                endPoint[0] = value[0]; 
                endPoint[1] = value[1];}
            }
    }
}
