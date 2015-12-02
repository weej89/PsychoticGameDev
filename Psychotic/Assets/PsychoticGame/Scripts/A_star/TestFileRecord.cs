#region Using
using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Threading;
#endregion

public static class TestFileRecord
{
	#region Private Variables
    private static StreamWriter testFile;
    private static string fileName;

    private static int entryNum = 0;
	#endregion

	#region CreateFile
	/// <summary>
	/// Creates the test file to hold results from pathfinding
	/// </summary>
	/// <param name="name">File name to be created or accessed.</param>
    public static void CreateFile(string name)
    {
        fileName = name;

		//Checks to see if the file exists and if it does then opens it
        if (File.Exists(fileName))
        {
            Debug.Log("File Already Exists.");
            testFile = new StreamWriter(fileName);
			testFile.WriteLine("Starting new RUN");
        }
		//Otherwise create the file and open it
        else
        {
            testFile = File.CreateText(fileName);
        }

		//Closes the test file to be used at a later time
        testFile.Close();
    }
	#endregion

	#region WriteToThatFile
	/// <summary>
	/// Writes to that file.
	/// This operation is thread safe
	/// </summary>
	/// <param name="text">The text to be entered into the file</param>
	/// <param name="lockObj">Thread handle</param>
    public static void WriteToThatFile(string text, object lockObj)
    {
        lock(lockObj)
        {
            testFile = File.AppendText(fileName);
            testFile.WriteLine(entryNum + ". - " +text);
            entryNum++;
            testFile.Close();
        }
    }
	#endregion

}
