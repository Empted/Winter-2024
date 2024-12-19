using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Entity
{
    public Entity(int x, int y, string type, int owner, int organId, string organDir, int organParentId, int organRootId)
    {
        this.x = x;
        this.y = y;
        this.type = type;
        this.owner = owner;
        this.organId = organId;
        this.organDir = organDir;
        this.organParentId = organParentId;
        this.organRootId = organRootId;
    }

    public int x {get;set;}
    public int y {get;set;}
    public string type { get; set; }
    public int owner { get; set; }
    public int organId { get; set; }
    public string organDir { get; set; }
    public int organParentId { get; set; }
    public int organRootId { get; set; }

    public int DistanceTo(Entity e)
    {
        return Util.Distance(x, e.x, y, e.y);
    }

}
public enum EntityType : int
{
    Wall = 0,
    Root = 1,
    Basic = 2,
    Tentacle = 3,
    Harvester = 4
}

class Util
{
    public static int Distance(int x1, int x2, int y1, int y2)
    {
        return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
    }
}
class Player
{


    static void Main(string[] args)
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        int width = int.Parse(inputs[0]); // columns in the game grid
        int height = int.Parse(inputs[1]); // rows in the game grid

        // game loop
        while (true)
        {
            var entities = new List<Entity>();
            int entityCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < entityCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int x = int.Parse(inputs[0]);
                int y = int.Parse(inputs[1]); // grid coordinate
                string type = inputs[2]; // WALL, ROOT, BASIC, TENTACLE, HARVESTER, SPORER, A, B, C, D
                int owner = int.Parse(inputs[3]); // 1 if your organ, 0 if enemy organ, -1 if neither
                int organId = int.Parse(inputs[4]); // id of this entity if it's an organ, 0 otherwise
                string organDir = inputs[5]; // N,E,S,W or X if not an organ
                int organParentId = int.Parse(inputs[6]);
                int organRootId = int.Parse(inputs[7]);
                var entity = new Entity(x, y, type, owner, organId, organDir, organParentId, organRootId);
                entities.Add(entity);
            }
            inputs = Console.ReadLine().Split(' ');
            int myA = int.Parse(inputs[0]);
            int myB = int.Parse(inputs[1]);
            int myC = int.Parse(inputs[2]);
            int myD = int.Parse(inputs[3]); // your protein stock
            inputs = Console.ReadLine().Split(' ');
            int oppA = int.Parse(inputs[0]);
            int oppB = int.Parse(inputs[1]);
            int oppC = int.Parse(inputs[2]);
            int oppD = int.Parse(inputs[3]); // opponent's protein stock
            int requiredActionsCount = int.Parse(Console.ReadLine()); // your number of organisms, output an action for each one in any order
            for (int i = 0; i < requiredActionsCount; i++)
            {
                var resourcesA = entities.Where(e => e.type == "A");
                var myOrgangs = entities.Where(e => e.owner == 1 && (e.type == "ROOT" || e.type == "BASIC"));

                var organResourceDistance = resourcesA.SelectMany(resource => myOrgangs.Select(organ => (res: resource, organ: organ, distance: organ.DistanceTo(resource))));
                if(organResourceDistance.Any())
                {
                    var best = organResourceDistance.MinBy(x => x.distance);
                    Console.WriteLine($"GROW {best.organ.organId} {best.res.x} {best.res.y} BASIC");
                }else
                {
                    var possibleCoords = myOrgangs.SelectMany(organ => new[] { ( organ.x + 1, organ.y, organ), (organ.x, organ.y + 1, organ), (organ.x - 1, organ.y, organ), (organ.x, organ.y -1, organ) });
                    var walls = entities.Where(e => e.type == "WALL");
                    possibleCoords.Where(coord => walls.All(w => w.x != coord.Item1 && w.y != coord.Item2) && coord.Item1 >= 0 && coord.Item1 < width && coord.Item2 >= 0 && coord.Item2 < height);
                    if(!possibleCoords.Any())
                    {
                        Console.WriteLine("WAIT");

                    }else
                    {
                        var best = possibleCoords.FirstOrDefault();
                        Console.WriteLine($"GROW {best.organ.organId} {best.Item1} {best.Item2} BASIC");
                    }
                }

            }
        }
    }
}