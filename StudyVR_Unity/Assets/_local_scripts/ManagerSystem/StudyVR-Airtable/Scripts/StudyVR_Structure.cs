using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region BASES
[System.Serializable]
public class STUDYVR_BASES
{
    public List<base_data> bases;
}

[System.Serializable]
public class base_data
{
    public string id;
    public string name;
    public string permissionLevel;
}
#endregion


#region TABLES
[System.Serializable]
public class STUDYVR_TABLES
{
    public List<table_data> tables;
}

[System.Serializable]
public class table_data
{
    public string id;
    public string name;
    public string primaryFieldId;
    public List<field_data> fields;
    public List<view_data> views;
}

[System.Serializable]
public class field_data
{
    public string type;
    public string id;
    public string name;
}

[System.Serializable]
public class view_data
{
    public string id;
    public string name;
    public string type;
}
#endregion



#region RECORDS
[System.Serializable]
public class STUDYVR_RECORDS
{
    public List<record_data> records;
}

[System.Serializable]
public class record_data
{
    public string id;
    public string createdTime;
    public fields_data fields;
}

[System.Serializable]
public class fields_data
{
    public string QuestionnaireID;
    public string QuestionnaireScene;
    public string PreStudyLink;
    public string MainStudyLink;
    public string AssetLink1;
    public string AssetLink2;
    public string Comments;
}
#endregion

