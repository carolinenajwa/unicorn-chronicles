﻿using System.Collections.Generic;
using System.IO;
using SQLite4Unity3d;
using UnityEngine;
#if !UNITY_EDITOR
#endif


/// <summary>
/// Represents a service for managing database operations related to questions.
/// </summary>
public class DataService
{
	private SQLiteConnection myConnection;


	/// <summary>
	/// Initializes a new instance of the DataService class with the specified database name.
	/// </summary>
	/// <param name="theDatabaseName">The name of the database.</param>
	public DataService(string DatabaseName)
	{
#if UNITY_EDITOR
		var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID 
            var loadDb =
 new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb =
 Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb =
 Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb =
 Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
		
#elif UNITY_STANDALONE_OSX
		var loadDb =
 Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb =
 Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
		myConnection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
		Debug.Log("Final PATH: " + dbPath);
	}


	/// <summary>
	/// Retrieves a sequence of all questions from the database.
	/// </summary>
	/// <returns>A sequence of all questions in the database.</returns>
	public IEnumerable<Question> GetQuestion()
	{
		return myConnection.Table<Question>();
	}

	/// <summary>
	/// Retrieves a sequence of all questions from the "Question" table in the database.
	/// </summary>
	/// <returns>A sequence of all questions from the "Question" table.</returns>
	public IEnumerable<Question> GetQuestions()
	{
		return myConnection.Query<Question>("SELECT * FROM Question");
	}

	/// <summary>
	/// Marks a specific question as answered in the database.
	/// </summary>
	/// <param name="theQuestionId">The ID of the question to mark as answered.</param>
	public void MarkQuestionAsAnswered(Question theQuestionId)
	{
		myConnection.Execute("UPDATE Question SET IsAnswered = 1 WHERE myQuestionID = ?", theQuestionId);
	}

	/// <summary>
	/// Resets the answered state of all questions in the database to unanswered.
	/// </summary>
	public static void ResetQuestionStateInDatabase()
	{
		// Assuming you've already created an instance of the DataService class
		DataService dataService = new DataService("data.sqlite");
		dataService.myConnection.Execute("UPDATE Question SET IsAnswered = 0");
	}

}
