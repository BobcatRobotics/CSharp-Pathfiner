// See https://aka.ms/new-console-template for more information

//Server: roborio-177-frc.local
//Table: SmartDashboard
/*
    NOTICE: HOW TO INPUT DATA
    
    Input ONLY numbers
    Separate by ONE space
    each new string in "args" is a new polygon pas the overall start and end points
    enter said numbers in this order:
    1. x val of overall start point
    2. y val of overall start point
    3. New string in args
    4. x val of overall end point
    5. y val of overall end point
    6. Steps a through c for each polygon
        a. New string in args
        b. x val of one polygon vertice
        c. y val of that same polygon vertice
*/


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
    public int argIndex;
    public static void Main(string[] args) {
        Program p = new Program();
        List<Polygon> presetPolygons = new List<Polygon>();
        p.argIndex = 0;

        
        //start point and end point (in the first and second arg strings)
        double[] sp = new double[] {p.ParseArgs(args[0]), p.ParseArgs(args[0])};
        p.argIndex = 0;
        double[] ep = new double[] {p.ParseArgs(args[1]), p.ParseArgs(args[1])};

        //create initial path
        Path iPath = new Path(sp, ep);

        List<Node> tempNodes = new List<Node>();

        //add each polygon from the given args (each string is a separate one)
        foreach (string s in args) {
            //while the index is less than the length (if less than or equal it would 
            //increase the index in the ParseArgs method past the array limit)
            while (p.argIndex < s.Length - 1) {
                //Add a new node to the tempnodes variable with the next 2 parsed nodes
                tempNodes.Add(new Node(p.ParseArgs(s), p.ParseArgs(s)));
            }
            //add a new polygon to the preset ones with the temp nodes
            presetPolygons.Add(new Polygon(tempNodes));
            //clear tempNodes
            tempNodes = new List<Node>();
            //reset argIndex for the next string
            p.argIndex = 0;
        }

        

        //Find paths (actual algorithm)
        List<Path> pathData = p.FindPaths(presetPolygons, iPath);
        //decode the data into pairs of coords and export it somehow
        double[,] exportData = p.GetPaths(pathData);
    }
    public List<Path> FindPaths(List<Polygon> p, Path initialPath) {
        List<Path> paths = new List<Path>();
        
        /*
        ACTUAL CODE FOR PATHFINDING
        
        RESUME WORK HEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEERE
        */

        //below is an <<<EXAMPLE>>> to prevent unassigned variable errors
        paths.Add(new Path(new double[] {0, 0}, new double[] {0, 0}));
        return paths;
    }
    //Call this from the outside
    public double[,] GetPaths(List<Path> paths) {
        //decode the list of paths into points
        //return statement just to get rid of error for now
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
    //parses a number from a spot in a string separated by spaces
    public double ParseArgs (string a) {
        string num = "";
        while (a[argIndex] != ' ') {
            num += a[argIndex];
            argIndex++;
        }
        //increase it again so it doesn't start on a space again
        argIndex++;
        return Double.Parse(num);
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
