using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//所有Prefab的标签
public enum UITAG
{
	UI_None,
	UI_MainMenu,
	UI_Cube,
	UI_Sphere,
	UI_Circle,
}
public enum OTHERTAG
{
	OTHER_None,
}

public class PrefabsPool 
{
	/// <summary>
	/// OtherPrefabsGameobject字典
	/// </summary>
	private Dictionary<OTHERTAG,GameObject> _OtherPrefabsDic = new Dictionary<OTHERTAG, GameObject>();

	private static PrefabsPool _Instance;
	public static PrefabsPool GetInstance()
	{
		if(null == _Instance)
		{
			_Instance = new PrefabsPool();
		}
		return _Instance;
	}

	// 将UITAG 和 对应的UIPrefab参数 绑定在一起  *UI预制体池
	public Dictionary<UITAG,UIPrefabParams> _UIPrefabBindingDic = new Dictionary<UITAG, UIPrefabParams>()
	{
		{UITAG.UI_None, 	new UIPrefabParams("")},
		{UITAG.UI_MainMenu, new UIPrefabParams("Prefabs/MainMenu")},
		{UITAG.UI_Cube, 	new UIPrefabParams("Prefabs/Cube",true)},
		{UITAG.UI_Sphere, 	new UIPrefabParams("Prefabs/Sphere",true)},
		{UITAG.UI_Circle, 	new UIPrefabParams("Prefabs/Circle",true)},
	};

	//将OTHERTAG 和 对应的其他Prefab参数 绑定在一起  *Other预制体池
	private Dictionary<OTHERTAG,OtherPrefabParams> _OtherPrefabsBindingDic = new Dictionary<OTHERTAG, OtherPrefabParams> ()
	{
		{OTHERTAG.OTHER_None, 	new OtherPrefabParams("")},
	};

	/// <summary>
	/// 加载不实例化
	/// </summary>
	public void LoadNotInstantiation(OTHERTAG otherTag){
		GameObject GameObj = Resources.Load (_OtherPrefabsBindingDic [otherTag].OtherPath) as GameObject;
		if(!_OtherPrefabsDic.ContainsKey(otherTag))
			_OtherPrefabsDic.Add (otherTag, GameObj);
	}
	/// <summary>
	/// 加载并实例化
	/// </summary>
	public void LoadAndInstantiation(OTHERTAG otherTag){
		Object Obj = Resources.Load (_OtherPrefabsBindingDic [otherTag].OtherPath);
		GameObject GameObj = GameObject.Instantiate (Obj) as GameObject;
		if(!_OtherPrefabsDic.ContainsKey(otherTag))
			_OtherPrefabsDic.Add (otherTag, GameObj);
		GameObj.name = Obj.name;
		GameObj.transform.parent = GameObject.Find ("UI Root").transform;
		GameObj.transform.localScale = Vector3.one;
	}
	/// <summary>
	/// 删除加载数据,不删除实例
	/// </summary>
	public void DestoryInDic(OTHERTAG otherTag){
		if (null != _OtherPrefabsDic) {
			if (_OtherPrefabsDic.ContainsKey (otherTag)) {
				_OtherPrefabsDic.Remove (otherTag);
			}
		} else {
			Debug.LogError("OtherPrefabs预制体池为空！");
		}
	}
	/// <summary>
	/// 清空所有加载数据
	/// </summary>
	public void DestroyAllDic(){
		if (null != _OtherPrefabsDic) {
			if (null != _OtherPrefabsDic) {
				_OtherPrefabsDic.Clear ();
			}
		} else{
			Debug.LogError("OtherPrefabs预制体池为空！");
		}
	}
}


//UIPrefab的参数
public class UIPrefabParams
{
	public UIPrefabParams(string uipath,bool NeedGoBack = false,bool NeedGoBackToMainMenu = false)
	{
		this.uipath = uipath;
		this.NeedGoBack = NeedGoBack;
		this.NeedGoBackToMainMenu = NeedGoBackToMainMenu;
	}
	//路径
	public string uipath ;
	//是否需要返回按钮
	public bool NeedGoBack ;
	//是否需要直接返回主菜单
	public bool NeedGoBackToMainMenu;
}

//其他Prefab的参数
public class OtherPrefabParams
{
	public OtherPrefabParams(string OtherPath)
	{
		this.OtherPath = OtherPath;
	}
	//路径 
	public string OtherPath;
}

