List<int>[] CreateAdjacencyMatrix(int numVecs) //Create a blank adj matrix for future manipulation
{
    List<int>[] matrix = new List<int>[numVecs]; //initialize at a specific size, creates numVecs number of rows
    for(int i = 0; i < numVecs; i++)
    {
        matrix[i] = new List<int>();        
    }
    return matrix;
}

void CreateEdge(List<int>[] matrix, int pt1, int pt2) //function that connects 2 vertices in the adj matrix
{
    if(pt1 != pt2) //quick fix to prevent overriding the -1 on the diagonal
    {
        matrix[pt1].Add(pt2); //If pt1 connects to pt2, then pt2 connects to pt1
        matrix[pt2].Add(pt1);
    }
}

void PrintMatrix(List<int>[] matrix) //outputs the contents of the current matrix to the screen
{
    Console.Write('\n'); // to make sure it starts on an empty line

    for(int i=0; i < matrix.Length; i++)
    {
        for(int j =0;j < matrix.Length; j++)
        {
            if(i == j) //have -1 on the diagonal
            {
                Console.Write(-1);
            }
            else if (matrix[i].Contains(j)) //if j is connected to i, then output 1
            {
                Console.Write(1);
            }
            else //otherwise, nothing is connected, so write a 0
            {
                Console.Write(0);
            }
            Console.Write(" ");
        }
        Console.Write('\n');
    }
}

void checkConnectedness(List<int>[] matrix) //check if any row is disconnected (All 0's)
{ //a row of all 0's implies a column of all 0's, therefore only need to check each row
    bool connected = true;
    for(int i = 0; i < matrix.Length; i++) //iterate through each row
    {
        bool row = matrix[i].All(x => x.Equals(0) || x.Equals(-1)); //a LINQ query to check every element in the row, if every single element in the row  is 0 or -1, then return true
        if(row) connected = false; //if a row exists that is all 0's and -1's, then that row is disconnected from the rest of the graph and then the graph is disconnected
    }

    if (connected) //output for easier readability
    {
        Console.WriteLine("The graph is connected");
    }
    else
    {
        Console.WriteLine("The graph is not connected");
    }
}

int[] countVertexDegrees(List<int>[] matrix) //count the degree of each vertex and put into an int array for later
{
    int[] degreeList = new int[matrix.Count()]; //initialize degreeList at the same size as the matrix we pass in
    for(int i=0; i < matrix.Length; i++)
    {
        int degree = matrix[i].Count(); //count the degree of each vertex
        degreeList[i] = degree;
    }
    return degreeList;
}

bool evenDegrees(int[] degreeList) //helper function to see if every degree in a vertex is even
{
    bool even = true;
    foreach(int degree in degreeList) //iterate through list
    {
        if(degree != 0)
        {
            if (degree % 2 != 0) //if there is a reminder after % 2, then we know it must be an odd number
            {
                even = false;
            }
        }
    }
    return even; //assuming that is even
}

void depthFirstSearch(bool[] visited, List<int> cycle, List<int>[] matrix, int vert)
    //my DFS interpretation
    //runs until matrix[vert] is empty, base case
{
    visited[vert] = true; //mark the vertex as visited
    cycle.Add(vert); //add it to the current Euler Cycle

    for(int i = 0; i< matrix[vert].Count(); i++) //iterate through each element left that is connected to the current vertex
    {
        int next = matrix[vert][i]; //find the next vertex
        if(valid(matrix, vert, next)) //see if removing this edge will be a valid move
        {
            matrix[vert].Remove(next); //remove the edge between this vertex and the next
            matrix[next].Remove(vert);
            depthFirstSearch(visited, cycle, matrix, next); //run this same function recursively with the next vertex
        }
    }
}

bool valid(List<int>[] matrix, int vert1, int vert2) //helper function to determine if removing the edge between two vertices will be valid
{
    if (matrix[vert1].Count == 1)
    {
        return true;
    }
    bool[] visited = new bool[matrix.Length]; //temp boolean array
    int countWith = CountDFS(vert1, matrix, visited); //measure depth with the vertices still connected

    matrix[vert1].Remove(vert2); //temp removal of current edge
    matrix[vert2].Remove(vert1);

    visited = new bool[matrix.Length];
    int countWithout = CountDFS(vert1, matrix, visited); //measure depth without the vertices being connected
    CreateEdge(matrix, vert1, vert2);
    return (countWith > countWithout) ? false : true; //ternary operator. If countWith > countWithout, then return false, else return true
}

int CountDFS(int vert, List<int>[] matrix, bool[] visited) //counter function. Rather than run my depth first search function that does extra work, this just counts the depth
{
    visited[vert] = true;
    int count = 1;
    foreach(int i in matrix[vert])
    {
        if (!visited[i])
        {
            count += CountDFS(i, matrix, visited); //still a DFS that counts. This increases the time complexity of the program unfortunately
        }
    }
    return count;
}

void printEuler(List<int> matrix) //Print the Cycle/Trail for any matrix we pass in
{
    foreach(var vertex in matrix) //iterate through
    {
       Console.Write(vertex + " - "); //Ends with one extra hyphen for now
    }
    Console.Write('\n'); // endline character
}

int startingVert(List<int>[] matrix) //determine what vertex to start at
{
    var x = countVertexDegrees(matrix);

    int numOdd = x.Count(x => x % 2 != 0); //find number of odd degrees

    if(numOdd == 0) //no odd vertices, then euler cycle possible
    {
        return 0; //start at vertex 0
    }
    if(numOdd == 1) //if 1 or more than 2, invalid
    {
        return -1;
    }
    if(numOdd == 2) // if 2, Euler trail possible.
    {
        for(int i=0;i<matrix.Count(); i++)
        {
            if (matrix[i].Count() % 2 != 0)
            {
                return i;
            }
        }
        return 0;
    }
    else
    {
        return -1;
    }
}

List<int> find(int numVecs, List<int>[] matrix) //initial function for calling the DFS function, tries to find the Euler Cycle/Trail
{
    bool[] visited = new bool[numVecs]; //init a blank boolean array of false values, the same # of elements as the vector array we create
    List<int> cycle = new List<int>(); //init a blank list<int> to hold the Euler cycle response

    int startingVect = startingVert(matrix); //find the starting vertex that our cycle will begin at. 0 or the first degree that's odd
    if (startingVect != -1) //-1 symbolizes an illegal value, therefore don't enter this loop if it can't work
    {
        for (int i = 0; i < matrix[startingVect].Count; i++)
        {
            depthFirstSearch(visited, cycle, matrix, startingVect);
        }
    }
    return cycle;
}

do //semi-permanent loop until user quits program
    //lets users run a few test functions or manually enter data as they see fit
{
    string input = "";
    Console.WriteLine("Enter 'quit' to end program.");
    Console.WriteLine("Enter 'func1' to find a Euler Cycle in a 5 vertex graph.");
    Console.WriteLine("Enter 'func2' to find a Euler Cycle in a 6 vertex graph.");
    Console.WriteLine("Enter 'func3' to find a Euler Trail in a 5 vertex graph");
    Console.WriteLine("Enter 'custom' to run custom test.");

    input = Console.ReadLine();
    switch (input.ToLower().Replace(" ", "")) //string parsing to normalize user input a bit
    {
        case "quit":
            return;

        case "func1":
            demoFunc1();
            break;

        case "func2":
            demoFunc2();
            break;

        case "func3":
            demoFunc3();
            break;

        case "custom":
            customInput();
            break;
        default:
            break;
    }
}
while (true);

    void demoFunc1() //demo function. Creates a 5 vertex connected graph.
    // -1 0 0 1 1
    // 0 -1 1 0 1
    // 0 1 -1 0 1
    // 1 0 0 -1 1
    // 1 1 1 1 -1
    //Graph has a Euler Cycle. 
    //Ex: 0 - 3 - 4 - 2 - 1 - 4 - 0
    {
        var matrix = CreateAdjacencyMatrix(5); //create initial matrix
        CreateEdge(matrix, 0, 4);
        CreateEdge(matrix, 1, 4);
        CreateEdge(matrix, 2, 4);
        CreateEdge(matrix, 3, 4); //add all the edges
        CreateEdge(matrix, 0, 3);
        CreateEdge(matrix, 1, 2);
        PrintMatrix(matrix); //print the matrix
        checkConnectedness(matrix); //check if connected
    if (evenDegrees(countVertexDegrees(matrix)))
    {
        Console.WriteLine("Euler Cycle is possible. Contains all vertices of even degree.");
    }
    else
    {
        Console.WriteLine("Euler Cycle impossible. Contains vertices of uneven degree");
    }
    List<int> cycle = find(5, matrix); //find the Euler Cycle
        printEuler(cycle); //print Euler Cycle
    }

    void demoFunc2() //demo function. Creates a 6 vertex connected graph
    // -1 1 1 0 0 0
    // 1 -1 1 1 1 0
    // 1 1 -1 1 1 0
    // 0 1 1 -1 1 1
    // 0 1 1 1 -1 1
    // 0 0 0 1 1 -1
    //contains a Euler cycle
    //Ex: 0 - 2 - 4 -5 - 3 - 2 - 1 - 4 - 3 - 1 - 0
    {
        var matrix = CreateAdjacencyMatrix(6);
        CreateEdge(matrix, 0, 1);
        CreateEdge(matrix, 0, 2);
        CreateEdge(matrix, 1, 2);
        CreateEdge(matrix, 1, 3);
        CreateEdge(matrix, 1, 4);
        CreateEdge(matrix, 2, 3);
        CreateEdge(matrix, 2, 4);
        CreateEdge(matrix, 3, 4);
        CreateEdge(matrix, 3, 5);
        CreateEdge(matrix, 4, 5);
        PrintMatrix(matrix);
        checkConnectedness(matrix);
    if (evenDegrees(countVertexDegrees(matrix)))
    {
        Console.WriteLine("Euler Cycle is possible. Contains all vertices of even degree.");
    }
    else
    {
        Console.WriteLine("Euler Cycle impossible. Contains vertices of uneven degree");
    }
    List<int> cycle = find(6, matrix);
        printEuler(cycle);
    }

    void demoFunc3() //demo function. Creates a 5 vertex connected graph with only Euler trail
    // -1 1 1 0 0
    // 1 -1 0 1 1
    // 1 0 -1 1 0
    // 0 1 1 -1 0
    // 0 1 0 0 -1
    //Contains no Euler Cycle, but contains Euler Trails
    //Ex: 1 - 0 - 2 - 3 - 1 - 4
{
    var matrix = CreateAdjacencyMatrix(5);
    CreateEdge(matrix, 0, 1);
    CreateEdge(matrix, 0, 2);
    CreateEdge(matrix, 1, 3);
    CreateEdge(matrix, 1, 4);
    CreateEdge(matrix, 2, 3);

    PrintMatrix(matrix);
    checkConnectedness(matrix);

    if (evenDegrees(countVertexDegrees(matrix)))
    {
        Console.WriteLine("Euler Cycle is possible. Contains all vertices of even degree.");
    }
    else
    {
        Console.WriteLine("Euler Cycle impossible. Contains vertices of uneven degree");
    }
    List<int> cycle = find(5, matrix);
    printEuler(cycle);

}

    void customInput() //this function allows a User to create their own graph
    {
        string done = "";

        Console.WriteLine("Please enter the number of vertices");
        string input = "";
        input = Console.ReadLine();
        int numVertices = 0;

        Int32.TryParse(input, out numVertices); //read user input to determine the number of vertices. Defaults to 0
        List<int>[] matrix = CreateAdjacencyMatrix(numVertices);
        int pt1 = -2, pt2 = -2; //initialize the pts being chosen as negative so we don't add them in later as that would cause an exception
        do //loops to add edges to the adj matrix
        {
            Console.WriteLine("Enter 'done' here to finish adding edges.");
            done = Console.ReadLine();

            if (done.ToLower().Replace(" ", "") == "done") //had to add an extra check here otherwise an exception would be thrown trying to hit the next line
            {
                break;
            }
            Console.WriteLine("Please enter vertex 1 to connect");
            Int32.TryParse(Console.ReadLine(), out pt1); //parsing function for parsing input to an in integer
            Console.WriteLine("Please enter vertex 2 to connect");
            Int32.TryParse(Console.ReadLine(), out pt2);
            if(pt1 >= 0 && pt2 >= 0) //to make sure we don't throw a bad value into the matrix
                {
                    CreateEdge(matrix, pt1, pt2);
                }
        } while (done.ToLower().Replace(" ", "") != "done");

        PrintMatrix(matrix); //output the newly created vertex
        checkConnectedness(matrix);

        if (evenDegrees(countVertexDegrees(matrix))) //check if every vertex in our matrix has an even degree therefore has a Euler Cycle
        {
            Console.WriteLine("Euler Cycle possible. Every vertex contains an even degree.");
        }
        else //search for a Euler Trail
        {
            Console.WriteLine("Euler Cycle not possible. Contains at least one vertex of an odd degree");
        //if Euler trail exists, output, else end
        }
        List<int> cycle = find(numVertices, matrix);
        printEuler(cycle); //print the newly discovered Cycle
}

//Citations:
// Our textbook, Section 2.1 Problems, #17
// initial idea of Fleury's Algorithm

// Anon. 1892. Journal de Mathématiques Spéciales. Rendiconti del Circolo Matematico di Palermo 6, S1 (1892), 58–59. DOI:http://dx.doi.org/10.1007/bf03017542  
// I might have cited the wrong article. I tried to cite the original French journal, but I cannot read French and can't tell if this is correct.

// Anon. 2023. Fleury’s algorithm for printing Eulerian Path or circuit. (February 2023). Retrieved May 15, 2023 from https://www.geeksforgeeks.org/fleurys-algorithm-for-printing-eulerian-path/ 
// helpful when looking into implementation of valid() function and how it works. 

// Anon.Retrieved May 15, 2023 from https://jlmartin.ku.edu/courses/math105-F11/Lectures/chapter5-part2.pdf 
// used for initial work on Fleury's Algorithm

// Lumen Learning David Lippman. Mathematics for the liberal arts. Retrieved May 15, 2023 from https://courses.lumenlearning.com/wmopen-mathforliberalarts/chapter/introduction-euler-paths/ 
// helpful for determining examples to use for euler cycles and paths. demoFunc3() uses their example as well for testing purposes.