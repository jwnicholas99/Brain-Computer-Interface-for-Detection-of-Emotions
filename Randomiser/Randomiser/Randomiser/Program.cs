using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Timers;
using System.Media;
using System.Configuration;
using System.Collections.Specialized;

namespace Randomiser
{
    class Program
    {
        static void Main(string[] args)
        {
            string src = "../pics";
            string[] folderList;
            //string dataFolder = "../../data/";
            int noOfFolders;
            int target = 12;
            int noOfBlocks = 20;
            int[][][] chosenFiles;
            //string[] chosenFileNames;
            //int[] shuffledFiles;
            string[][] fileList;
            int[][] randomizedEven;
            int[][] randomizedOdd;
            int[][] shuffledEven;
            int[][] shuffledOdd;
            int totalNoBlocks;
            int totalNoPS;
            //int totalNoPics;
            //string indexNumber;
            //string dataPath;
            //string imgPath;
            //int fileIndex;
            string shuffledPathEven;
            string shuffledPathOdd;
            string fileNameEven;
            string fileNameOdd;

            //gets the names of folders
            folderList = Directory.GetDirectories(src); //returns string array
            noOfFolders = folderList.Length;
            fileList = new string[noOfFolders][];
            chosenFiles = new int[noOfFolders][][];

            for (int l = 0; l < noOfFolders; l++)
            {
                //preparing the folderList for each folder
                string[] filesInList = Directory.GetFiles(folderList[l]); //returns string array too, with names of each file in an array
                int noOfFiles = filesInList.Length;
                fileList[l] = filesInList; // contains FILENAMES
                chosenFiles[l] = new int[noOfBlocks][];

                //selecting 12 pictures for each block (there will be repeated values)
                for (int i = 0; i < noOfBlocks; i++)
                {
                    //chosing target number of files randomly from total files in each folder
                    int[] chosenNos = select(noOfFiles, target);

                    for (int p = 0; p < target; p++)
                    {
                        chosenNos[p] = chosenNos[p] + 100 * l;
                    }

                    chosenFiles[l][i] = new int[target];
                    //3d array with: e.g. block [1] folder [1] containing [3][12][15] of randomly picked NUMBERS from 1st folder
                    chosenFiles[l][i] = chosenNos;
                }



            }
       
            //Randomizing the blocks (1st 6 blocks in a class for 1st session, 2nd 6 blocks in a class for 2nd session)
            //Misnaming even and odd here- we realised that even and odd was messier than first and last 6 to calculate
            totalNoBlocks = noOfFolders * noOfBlocks;
            totalNoPS = totalNoBlocks / 2; // because we have 2 sessions
            int noOfBlocksPCPS = noOfBlocks / 2; // total number of blocks per class per session = 10

            randomizedEven = new int[totalNoPS][];
            for (int i = 0; i < noOfFolders; i++)
            {
                for (int j = 0; j < noOfBlocksPCPS; j++)
                {
                    randomizedEven[i * noOfBlocksPCPS + j] = new int[target];
                    randomizedEven[i * noOfBlocksPCPS + j] = chosenFiles[i][j];
                }

            }
            shuffledEven = shuffle(randomizedEven);


            randomizedOdd = new int[totalNoPS][];

            for (int i = 0; i < noOfFolders; i++)
            {
                for (int j = noOfBlocksPCPS; j < noOfBlocks; j++)
                {
                    randomizedOdd[i * noOfBlocksPCPS + j - noOfBlocksPCPS] = new int[target];
                    randomizedOdd[i * noOfBlocksPCPS + j - noOfBlocksPCPS] = chosenFiles[i][j];
                }

            }
            shuffledOdd = shuffle(randomizedOdd);



            //Create Files to store data
            string timeStamp = DateTime.Now.ToString("MMddHHmmss");
            fileNameEven = timeStamp  + "Session1.txt";
            fileNameOdd = timeStamp + "Session2.txt";
            shuffledPathEven = Path.Combine("../shuffledTexts", fileNameEven);
            shuffledPathOdd = Path.Combine("../shuffledTexts", fileNameOdd);

            //Creates a file for Session 1
            for (int i = 0; i <  totalNoPS ; i++)
            {
                for (int k = 0; k < target; k++)
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(shuffledPathEven, true))
                    { 
                        file.WriteLine(fileList[shuffledEven[i][k]/100][shuffledEven[i][k]%100].ToString());
                    }
                }   
            }

            //Creates a file for Session 2
            for (int i = 0; i < totalNoPS; i++)
            {
                for (int k = 0; k < target; k++)
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(shuffledPathOdd, true))
                    {
                        file.WriteLine(fileList[shuffledOdd[i][k] / 100][shuffledOdd[i][k]%100].ToString());
                    }
                }
            }





        }






        //place a total number of files, and this function will select a target number of files
        private static int[] select(int totalNo, int target)
        {
            int[] totalNos = new int[totalNo];
            int[] chosenNos = new int[target];
            for (int i = 0; i < totalNo; i++)
            {
                totalNos[i] = i;
            }
            for (int t = 0; t < target; t++)
            {
                int r = RandomNumber(0, totalNo - t); 
                chosenNos[t] = totalNos[r];
                totalNos[r] = totalNos[totalNo - t - 1];
            }

            return chosenNos;
        }




        

        //shuffles a list
        private static int[][] shuffle(int[][] shuffledNos)
        {

            for (int u = 0; u < 10000; u++)
            {
                for (int t = 0; t < shuffledNos.Length; t++)
                {
                    int Seed = (int)DateTime.Now.Ticks;
                    Random rnd = new Random(Seed);
                    int r = rnd.Next(1, shuffledNos.Length);

                    int[] temporaryValue = shuffledNos[t];
                    shuffledNos[t] = shuffledNos[r];
                    shuffledNos[r] = temporaryValue;
                }
            }
            return shuffledNos;
        }





        private static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            object syncLock = new object();
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }




    }
}
