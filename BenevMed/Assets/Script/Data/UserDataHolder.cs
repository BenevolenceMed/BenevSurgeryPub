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
        ExpertMode = false;     // should read data from somewhere else ?

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
    }

    void OnRecentLearningUpdated(object data)
    {
        Debug.Log("Last Learning Updated.. " + (string)data);
        LastRecentLearning = (string)data;
    }




    public void AddNote(string surgName, NoteData data)
    {
        if (DictNotes.ContainsKey(surgName))
            DictNotes[surgName].Add(data);
        else
        {
            List<NoteData> listNotes = new List<NoteData>();
            listNotes.Add(data);
            DictNotes.Add(surgName, listNotes);
        }
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
        return true;
    }
}