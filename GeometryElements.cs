using System;

public class Polygon {
    public List<Node> vertices;
    public Node center;
    public List<Line> lines;
    public double[] ranges;
    public Polygon (List<Node> v) {
        vertices = v;
        //temp variables for below
        double xtotal = 0;
        double ytotal = 0;

        //set center to average of all x and y values of the polygon's vertices
        foreach (Node p in v) {
            xtotal += p.x;
            ytotal += p.y;
        }
        center = new Node(xtotal / v.Count, ytotal / v.Count);

        //set "lines" to be the lines from the center to each point
        foreach (Node n in vertices) {
            lines.Add(new Line(n, center));
        }

        //Set a sort of bounding box for the Polygon (so you don't have to pathfind with any polygons outside the 
        //journey base)

        //mins and maxes replaced later, just set them as this to remove "accessing unassigned variables" errors
        double xMin = vertices[0].x;
        double xMax = vertices[0].x;
        double yMin = vertices[0].y;
        double yMax = vertices[0].y;
        foreach (Node n in vertices) {
            if (n.x < xMin) {
                xMin = n.x;
            }
            if (n.x > xMax) {
                xMax = n.x;
            }
            if (n.x < xMin) {
                yMin = n.y;
            }
            if (n.y > yMax) {
                yMax = n.y;
            }
        }
        ranges = new double[] {xMin, yMin, xMax, yMax};
    }

}
public class Node {
    public double x;
    public double y;
    public Node (double x, double y) {
        this.x = x;
        this.y = y;
    }
}
public struct Line {
    public double[] xrange;
    public double slope;
    public bool slopeIsUndefined;
    //b value
    public double intercept;
    public double length;

    public Node start;
    public Node end;
    public Line (Node startPoint, Node endPoint) {
        start = startPoint;
        end = endPoint;
        if (startPoint.x == endPoint.x) {
            //if the slope is infinite/negative infinite
            slopeIsUndefined = true;
        }   
        slope = (startPoint.y - endPoint.y)/(startPoint.x - endPoint.x);
        //y = mx + b turns into y - mx = b
        intercept = startPoint.y - slope * startPoint.x;

        //set range of values for the line equation (where it cuts off)
        //checks which is lower and puts it last so it is a proper range
        if (startPoint.x <= endPoint.x) {
            xrange = new double[] {startPoint.x, endPoint.x};
        } else {
            xrange = new double[] {endPoint.x, startPoint.x};
        }
        length = Math.Sqrt(Math.Pow(startPoint.y - endPoint.y, 2) + Math.Pow(startPoint.y - endPoint.y, 2));
    }
}
public class Path {
    public Path (Node sP, Node eP) {
        StartPoint = sP;
        EndPoint = eP;
    }
    private Node startPoint;
    public Node StartPoint {
        get; set;
    }
    private Node endPoint;
    public Node EndPoint {
        get; set;
    }
}
public class Route {
    public List<Path> paths;
    public List<Line> lines;
    public Route (List<Path> p)
    {
        paths = p;
        lines = new List<Line>();
        foreach (Path pa in p) {
            lines.Add(new Line(pa.StartPoint, pa.EndPoint));
        }
    }
    public Route (List<Line> l) {
        lines = l;
        paths = new List<Path>();
        foreach (Line la in lines) {
            lines.Add(new Line(la.start, la.end));
        }
    }
    //argumentless constructor for just adding paths and not immediately initializing them
    public Route() {
        paths = new List<Path>();
        lines = new List<Line>();
    }
}

