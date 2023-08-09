using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benev.Events;

public class NoteData
{
    // public string SurgName { get; set; }
    public float fTimeRate { get; set; }    // 0 ~ 1.0f
    public string Content { get; set; }
}

public class UserDataHolder
{
    public bool ExpertMode { get; set; }            // false to Beginner Mode.
    public string LastRecentLearning { get; set; }  // 
    public string CurrentLearning { get; set; }
    public Dictionary<string, List<NoteData>> DictNotes = new Dictionary<string, List<NoteData>>();

    EventsGroup Events = new EventsGroup();

    public void Init()
    {
        Load();

        Events.RegisterEvent("OnSurgSelected", OnSurgSelected);
        Events.RegisterEvent("OnExpertModeChanged", OnExpertModeChanged);
        Events.RegisterEvent("OnSurgSectionItemClicked", OnRecentLearningUpdated);
    }


    void OnSurgSelected(object data)
    {
        Debug.Log("Current Learning Updated.. " + (string)data);
        CurrentLearning = (string)data;
    }

    void OnExpertModeChanged(object data)
    {
        Debug.Log("Mode has been changed.");
        ExpertMode = (int)data == 1 ? true : false;
        PlayerPrefs.SetInt("ExpertMode", ExpertMode ? 1 : 0);
    }

    void OnRecentLearningUpdated(object data)
    {
        Debug.Log("Last Learning Updated.. " + (string)data);
        LastRecentLearning = (string)data;
        PlayerPrefs.SetString("LastRecentLearning", LastRecentLearning);
    }

    #region Data Load / Save
    public void Load()
    {
        ExpertMode = PlayerPrefs.GetInt("ExpertMode", 0) == 1 ? true : false;
        LastRecentLearning = PlayerPrefs.GetString("LastRecentLearning", "");

        LoadNoteData();
    }
    public void LoadNoteData()
    {
        DictNotes.Clear();

        string keyInfo = PlayerPrefs.GetString("NoteKeyInfo", "");
        if (keyInfo.Length == 0) return;

        string[] singleKeyInfo = keyInfo.Split('/');
        for (int k = 0; k < singleKeyInfo.Length; ++k)
        {
            string[] info = singleKeyInfo[k].Split(':');
            if (info.Length != 2) continue;

            int size = int.Parse(info[1]);
            for (int q = 0; q < size; ++q)
            {
                string noteInfo = PlayerPrefs.GetString($"NoteData-{info[0]}-{q}", "");
                if (noteInfo.Length == 0)
                    continue;
                string[] noteData = noteInfo.Split(':');
                if (noteData.Length != 2)
                    continue;

                NoteData data = new NoteData();
                data.fTimeRate = float.Parse(noteData[0]);
                data.Content = noteData[1];
                AddNote(info[0], data, false);
            }
        }
    }

    public void SaveNoteData()
    {
        string keyInfo = "";
        foreach (string key in DictNotes.Keys)
        {
            if (DictNotes[key].Count == 0)
                continue;

            keyInfo += $"{key}:{DictNotes[key].Count}" + "/";
        }
        keyInfo = keyInfo.Remove(keyInfo.Length - 1);
        PlayerPrefs.SetString("NoteKeyInfo", keyInfo);


        foreach (string key in DictNotes.Keys)
        {
            if (DictNotes[key].Count == 0)
                continue;

            List<NoteData> listData = DictNotes[key];
            for (int k = 0; k < listData.Count; ++k)
            {
                PlayerPrefs.SetString($"NoteData-{key}-{k}", $"{listData[k].fTimeRate}:{listData[k].Content}");
            }
        }
    }
    #endregion


    #region Note Data Handling.
    public void AddNote(string surgName, NoteData data, bool save=true)
    {
        if (DictNotes.ContainsKey(surgName))
            DictNotes[surgName].Add(data);
        else
        {
            List<NoteData> listNotes = new List<NoteData>();
            listNotes.Add(data);
            DictNotes.Add(surgName, listNotes);
        }

        if(save)    SaveNoteData();
    }
    public bool RemoveNote(string surgName, float fRate)
    {
        if (!DictNotes.ContainsKey(surgName))
            return false;

        List<NoteData> listNote = DictNotes[surgName];
        for (int k = 0; k < listNote.Count; ++k)
        {
            if (Mathf.Abs(listNote[k].fTimeRate - fRate) <= Mathf.Epsilon)
            {
                listNote.RemoveAt(k);
                if (listNote.Count == 0)
                    DictNotes.Remove(surgName);

                SaveNoteData();
                return true;
            }
        }
        return false;
    }
    public NoteData GetNote(string surgName, float fRate)
    {
        int idx = GetNoteIndex(surgName, fRate);
        if (idx < 0) return null;

        List<NoteData> listNote = DictNotes[surgName];
        if (idx < listNote.Count)
            return listNote[idx];
        return null;
    }
    public NoteData GetNoteByIndex(string surgName, int idx)
    {
        if (!DictNotes.ContainsKey(surgName))
            return null;

        if (DictNotes[surgName].Count <= idx)
            return null;

        return DictNotes[surgName][idx];
    }
    public int GetNoteIndex(string surgName, float fRate)
    {
        if (!DictNotes.ContainsKey(surgName))
            return -1;

        List<NoteData> listNote = DictNotes[surgName];
        for (int k = 0; k < listNote.Count; ++k)
        {
            if (Mathf.Abs(listNote[k].fTimeRate - fRate) <= Mathf.Epsilon)
                return k;
        }
        return -1;
    }
    public bool UpdateNote(string surgName, NoteData newData)
    {
        NoteData data = GetNote(surgName, newData.fTimeRate);
        if (data == null)
            return false;

        data.fTimeRate = newData.fTimeRate;
        data.Content = newData.Content;

        SaveNoteData();
        return true;
    }
    #endregion
}