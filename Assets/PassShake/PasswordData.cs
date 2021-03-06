﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

namespace PassShake
{
    class PasswordData
    {
        public bool passwordExists = false;

        private List<float[][][]> handGestures;

        public PasswordData(string passwordLocation)
        {
            handGestures = new List<float[][][]>();

            loadPassword(passwordLocation);
        }

        public void loadPassword(string passwordLocation)
        {
            if (File.Exists(passwordLocation))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(passwordLocation, FileMode.Open)))
                {
                    int length = reader.ReadInt32();
                    for(int k = 0; k < length; k++)
                    {
                        float[][][] gesture = new float[2][][];
                        gesture[0] = new float[6][];
                        gesture[1] = new float[6][];
                        for (int i = 0; i < 6; i ++)
                        {
                            gesture[0][i] = new float[3];
                            gesture[1][i] = new float[3];
                            for(int j = 0; j < 3; j ++)
                            {
                                gesture[0][i][j] = reader.ReadSingle();
                                gesture[1][i][j] = reader.ReadSingle();
                            }
                        }
                        handGestures.Add(gesture);
                    }
                    reader.Close();
                }
                passwordExists = true;
            }
        }

        public void write(string passwordLocation, List<float[][][]> data)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(passwordLocation, FileMode.Create)))
            {
                int length = data.Count;
                writer.Write(length);
                for(int i = 0; i < length; i++)
                {
                    for(int j = 0; j < 6; j++)
                    {
                        for(int k = 0; k < 3; k++)
                        {
                            writer.Write(data[i][0][j][k]);
                            writer.Write(data[i][1][j][k]);
                        }
                    }
                }
                writer.Close();
            }
            handGestures = data;
        }

        public float[][] getRightHandGesture(int index)
        {
            return handGestures[index][0];
        }

        public float[][] getLeftHandGesture(int index)
        {
            return handGestures[index][1];
        }

        public List<float[][][]> getHandGesture()
        {
            return handGestures;
        }
    }
}
