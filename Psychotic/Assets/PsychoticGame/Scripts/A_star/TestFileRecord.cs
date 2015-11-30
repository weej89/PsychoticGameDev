using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Threading;

public static class TestFileRecord
{
    private static StreamWriter testFile;
    private static string fileName;

    private static int entryNum = 0;

    public static void CreateFile(string name)
    {
        fileName = name;

        if (File.Exists(fileName))
        {
            Debug.Log("File Already Exists.");
            testFile = new StreamWriter(fileName);
        }
        else
        {
            testFile = File.CreateText(fileName);
        }

        testFile.Close();
    }

    public static void WriteToThatFile(string text, object lockObj)
    {
        lock(lockObj)
        {
            testFile = new StreamWriter(fileName);
            testFile.WriteLine(entryNum + ". - " +text);
            entryNum++;
            testFile.Close();
        }
    }


}
