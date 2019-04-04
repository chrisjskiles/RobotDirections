using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotDirections {
    class Program {
        static void Main(string[] args) {
            string filePath = String.Empty;

            //get path of file with word declarations
            Console.Write("Please enter file path: ");
            filePath = Console.ReadLine();

            //exit application if file does not exist
            if (!File.Exists(filePath)) {
                Console.WriteLine("No such file exists.");
                Console.Read();

                return;
            }


            //each line we split the date, location and instructions from each other
            using (StreamReader reader = new StreamReader(filePath)) {
                string line = String.Empty;

                while ((line = reader.ReadLine()) != null) {
                    string[] splitLine = line.Split(';');

                    splitLine[2] = splitLine[2].Replace(" ", "");   //remove whitespace

                    string shortestPath = FindShortestPath(new List<string>(splitLine[2].Split(',')));

                    Console.WriteLine(String.Format("{0};{1};{2}", splitLine[0], splitLine[1], shortestPath));  //print results
                }
            }

            Console.Read();
        }

        //given a list of instructions find the shortest possible route
        public static string FindShortestPath(List<string> directions) {
            Compass currentHeading = Compass.West;  //robot's heading - always starts facing west

            //number of blocks traveled in the east/west and north/south directions with east and north being positive
            int eastWest = 0;
            int northSouth = 0;

            foreach (string direction in directions) {

                //change robot's heading based on the direction turned
                if (direction[0] == 'R') {
                    currentHeading = (Compass)(((int)currentHeading + 1) % 4);
                }

                else {
                    currentHeading = (Compass)(((int)currentHeading - 1) % 4);
                }

                //add or subtract distance based on current heading
                int distance = Convert.ToInt32(direction.Substring(1));

                switch (currentHeading) {
                    case Compass.North:
                        northSouth += distance;
                        break;

                    case Compass.South:
                        northSouth -= distance;
                        break;

                    case Compass.East:
                        eastWest += distance;
                        break;

                    case Compass.West:
                        eastWest -= distance;
                        break;
                }
            }

            //calculate shortest path
            string shortestPath = String.Empty;

            //handle north/south translation and set direction for future reference
            if (northSouth < 0) {
                shortestPath += "L" + Math.Abs(northSouth);
                currentHeading = Compass.South;
            }

            else if (northSouth > 0) {
                shortestPath += "R" + northSouth;
                currentHeading = Compass.North;
            }

            //if no north/south translation, simply face the direction and head that way
            else {
                if (eastWest < 0) {
                    return "R0,L" + Math.Abs(eastWest);   //since there is no instruction to go straight, if you need to head west you must turn once first
                }
                
                else if (eastWest > 0) {
                    return "R0,R" + Math.Abs(eastWest); //do a 180
                }
            }


            //handle east/west translation
            if ((currentHeading == Compass.North && eastWest < 0) || (currentHeading == Compass.South && eastWest > 0)) {
                shortestPath += ",L" + Math.Abs(eastWest);
            }            

            else if (eastWest != 0) {
                shortestPath += ",R" + Math.Abs(eastWest);
            }

            return shortestPath;
        }

        //enum to track the direction the robot is facing
        public enum Compass {
            North = 0,
            East = 1, 
            South = 2,
            West = 3
        }
    }
}
