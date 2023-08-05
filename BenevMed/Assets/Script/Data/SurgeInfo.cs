using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SurgeInfo
{
    public int Category;
    public string Name;
    public string Desc;
}

[Serializable]
public class SurgeListInfo
{
    public List<SurgeInfo> SurgeryLists;
}







[Serializable]
public class SurgeSectionInfo
{
    public string Name;
    public string Desc;
}


[Serializable]
public class SurgeDetailInfo
{
    public List<SurgeSectionInfo> SectionLists;
}
