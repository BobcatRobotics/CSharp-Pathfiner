// See https://aka.ms/new-console-template for more information

//Note to self (networktables):
//Server: roborio-177-frc.local
//Table: SmartDashboard

/*
    HOW TO INTERPRET EXPORTED DATA:

    The exported data is in a 2D double array, with each inner array having two values: the x and the y of each coord
    (respectively). These coords form the start point of all the paths, the vertices connecting them, and the end point
    of them all. The start point should be the Robot's current position, so I'm keeping it in for testing (and because
    I'm too lazy to change it)
*/

/*
    TO DO:

    - Networktables
        - Set up a newtork table
        - Publish to a network table
        - Implement code taking in data from networktable
    - Algorithms
        - Pathfinding
            - Finding lines the original journey intersects
            - Finding lines each journey that comes afterward intersects
            - Get all possible paths
            - Brute force the best paths (least distance)
            - Get new path between the closest point to the endpoint and the actual endpoint
            - Repeating this
    - Testing
        - Node creation from args
        - Polygon creation from args
        - Finding shortest path
        - Decoding path data
        - Sending correct data to a networktable.
    - Optimizing
        - Clean up code format
        - Remove unecessary comments
        - Remove unecessary variables/reduce memory
        - <Other stuff I'm never gonna end up doing>

*/

using System;
//NOTE: OTHER CLASSES AND STUFF ALREADY ACCESSED FROM THE GEOMETRYELEMENTS FILE - NO NEED TO IMPORT/SAY "using ____;"

public class Program {
    private double total;
    public List<Path> paths;
    public int argIndex;
    public static void Main(string[] args) {
        Program p = new Program();
        List<Polygon> presetPolygons = new List<Polygon>();
        p.argIndex = 0;

        
        //start point and end point (in the first and second arg strings)
        Node sp = new Node(p.ParseArgs(args[0]), p.ParseArgs(args[0]));
        p.argIndex = 0;
        Node ep = new Node(p.ParseArgs(args[1]), p.ParseArgs(args[1]));

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
    //only have polygons passed in be ones that have a CHANCE of being intersected: Like, they're not all the way on the
    //other side of the field
    public List<Path> FindPaths(List<Polygon> polys, Path initialPath) {
        List<Path> paths = new List<Path>();
        
        /*
        ACTUAL CODE FOR PATHFINDING
        
        
        */
        List<Polygon> intersectedPolygons = new List<Polygon>();
        //the path stemming from the end of each polygon to the endPoint (run this one through the algorithm)
        Line pathToEndpoint = new Line(initialPath.StartPoint, initialPath.EndPoint);
        List<Node> possiblePathVertices = new List<Node>();
        List<Line> possibleLines = new List<Line>();
        foreach (Polygon p in polys) {
            //execute pathfinding steps for each polygon here
            possiblePathVertices.Add(initialPath.StartPoint);
            possiblePathVertices.Add(initialPath.EndPoint);
            for (int i = 0; i < p.lines.Count - 1; i++) {
                //the lines were created based on each vertice in a foreach loop, meaning the connected ones share the 
                //same index in their respective arrays
                if (LinesIntersecting(p.lines[i], pathToEndpoint)) {
                    possiblePathVertices.Add(p.vertices[i]);
                }
            }
            //add each combination of lines to the possible lines (if they don't go through the polygon itself)
            
            //for each combination of 2 nodes in the possible path vertices...
            for (int i = 0; i < possiblePathVertices.Count; i++) {
                for (int j = 0; j < possiblePathVertices.Count; j++) {
                    //...if they don't equal each other...
                    if (i != j) {
                        ///...check if a line between them goes through the polygon...
                        Line l = new Line (possiblePathVertices[i], possiblePathVertices[j]);
                        bool goingThroughPolygon = false;
                        foreach (Line li in p.lines) {
                            //so goingThroughPolygon is set true and stays true (the break statement)
                            if (LinesIntersecting(li, l)) {
                                
                                goingThroughPolygon = true;
                                break;
                            }
                        }
                        if (!goingThroughPolygon) {
                            //make a new one instead of using l because l is temporary
                            possibleLines.Add(new Line(l.start, l.end));
                            
                        }
                        
                    }
                    
                }   
            }

        }

        //below is an <<<EXAMPLE>>> to prevent unassigned variable errors
        paths.Add(new Path(new Node(0, 0), new Node (0, 0)));
        return paths;
    }
    //Call this from the outside
    public double[,] GetPaths(List<Path> paths) {
        //create a new 2D array representing coords with the amount of them being the # of paths + 1 (because one at each
        //path's end + one at the first one's beginning: ._._._. 4 nodes, 3 lines)
        double[,] coords = new double[paths.Count + 1, 2];

        //set the first coords to be the starting path's startpoint
        coords[0, 0] = paths[0].StartPoint.x;
        coords[0, 1] = paths[0].StartPoint.y;

        //set the rest of the coords to be the rest of the paths' endpoints
        for (int i = 0; i < paths.Count - 1; i++) {
            coords[i, 0] = paths[i].EndPoint.x;
            coords[i, 1] = paths[i].EndPoint.y;
        }
        //decode the list of paths into points
        //return statement just to get rid of error for now
        return coords;
    }
    public void sumLengths (List<Line> currentLines, List<Line> connectingLines) {
        //recurse this function through all connecting lines to find the fastest combination for each branch
        foreach (Line l in currentLines) {
            //adds all the lines connecting to the "current" line's end
            List<Line> nextLines = new List<Line>();
            foreach (Line ln in connectingLines) {
                //don't need to check if they are different because noline has the same start and end points
                if (ln.start == l.end) {
                    nextLines.Add(ln);
                }
            }
            //take all those lines that are connected and do the same (if not the end)
            if (nextLines.Count != 0) {
                
                sumLengths(nextLines, connectingLines);
            }
            else {
                //when it reaches the end
                total += l.length;
            }
            //RESUME AROUND HEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEERE
        }
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
