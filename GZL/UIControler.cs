using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//UI控制类
public class UIControler : MonoBehaviour 
{
	/// UIGameObject字典 
	private Dictionary<UITAG,GameObject> _UIGameobjDic = new Dictionary<UITAG, GameObject> ();
	/// LastUITAG字典
	private Dictionary<UITAG,UITAG> _LastUITAGDic = new Dictionary<UITAG, UITAG> ();
	/// 上一个UITAG
	public UITAG LastUITAG = UITAG.UI_None;
	/// 现在的UITAG
	public UITAG NowUITAG = UITAG.UI_None;

	/// 单例
	private static UIControler _instance = null;
	public static UIControler GetInstance()
	{
		return _instance;
	}
	void Awake()
	{
		_instance = this;
		PopUpUI (UITAG.UI_MainMenu);
	}

	/// <summary>
	/// 弹出UI界面
	/// </summary>
	public void PopUpUI(UITAG uitag)
	{
		if(uitag != UITAG.UI_None)
		{
			if(uitag == UITAG.UI_MainMenu)
			{
				_LastUITAGDic.Clear();
			}
			if (!_UIGameobjDic.ContainsKey (uitag))
			{
				Object obj = Resources.Load (PrefabsPool.GetInstance()._UIPrefabBindingDic [uitag].uipath);
				GameObject Gameobj = GameObject.Instantiate (obj) as GameObject;
				Gameobj.name = obj.name;
				Gameobj.transform.parent = GameObject.Find ("UI Root").transform;
				Gameobj.transform.localScale = Vector3.one;
				if(!_UIGameobjDic.ContainsKey(uitag))
					_UIGameobjDic.Add (uitag, Gameobj);

				if(PrefabsPool.GetInstance()._UIPrefabBindingDic[uitag].NeedGoBack)
				{
					InstantiateGoBack(Gameobj);
				}

				LastUITAG = NowUITAG;
				NowUITAG = uitag;

				if(!_LastUITAGDic.ContainsKey(uitag))
				{
					_LastUITAGDic.Add(uitag,LastUITAG);
				}

				Debug.Log(uitag + "    加载成功！");
			}

		}
		else
		{
			Debug.LogError(uitag);
		}
	}

	/// <summary>
	/// 删除UI界面
	/// </summary>
	public void DeleteUI(UITAG uitag)
	{
		if(uitag != UITAG.UI_None)
		{
			if (!_UIGameobjDic.ContainsKey (uitag))
				Debug.LogError (uitag + "    不存在!");
			else
			{
				GameObject Gameobj = _UIGameobjDic [uitag];
				if (Gameobj != null)
				{
					_UIGameobjDic.Remove(uitag);
					Destroy (Gameobj);
					Debug.Log(uitag + "    删除!");
				}
			}
		}
		else
		{
			Debug.LogError(uitag);
		}
	}

	/// <summary>
	/// 从这个界面 跳到 下一个界面
	/// </summary>
	public void GoToNextUI(UITAG uitagto)
	{
		if (uitagto != UITAG.UI_None) 
		{
			if(uitagto == UITAG.UI_MainMenu)
			{
				Debug.LogWarning("最好选择方法：GoToMainMenu（）来返回主菜单！");
			}
			DeleteUI (NowUITAG);
			PopUpUI (uitagto);
		}
		else
		{
			Debug.LogError(uitagto);
		}
	}

	/// <summary>
	/// 删除所有界面，直接跳到主菜单
	/// </summary>
	public void GoToMainMenu()
	{
		if(_UIGameobjDic.Count > 0)
		{
			foreach(KeyValuePair<UITAG,GameObject> DicObj in _UIGameobjDic)
			{
				GameObject GameObj = DicObj.Value;
				if(GameObj != null)
				{
					Destroy (DicObj .Value);
					Debug.Log(DicObj.Key + "    删除!");
				}
				else
					Debug.LogError("Value为空!");
			}
			_UIGameobjDic.Clear();
		}
		PopUpUI(UITAG.UI_MainMenu);
		
		LastUITAG = UITAG.UI_None;
		NowUITAG = UITAG.UI_MainMenu;
	}
	
	/// <summary>
	/// 返回上一个UI界面
	/// </summary>
	public void GoBack()
	{
		if(PrefabsPool.GetInstance()._UIPrefabBindingDic [NowUITAG].NeedGoBackToMainMenu)
		{
			GoToMainMenu();
		}
		else
		{
			if (LastUITAG != UITAG.UI_None) 
			{
				DeleteUI (NowUITAG);
				if(LastUITAG != UITAG.UI_None)
				{
					if (!_UIGameobjDic.ContainsKey (LastUITAG))
					{
						Object obj = Resources.Load (PrefabsPool.GetInstance()._UIPrefabBindingDic [LastUITAG].uipath);
						GameObject Gameobj = GameObject.Instantiate (obj) as GameObject;
						Gameobj.name = obj.name;
						Gameobj.transform.parent = GameObject.Find ("UI Root").transform;
						Gameobj.transform.localScale = Vector3.one;
						_UIGameobjDic.Add (LastUITAG, Gameobj);
						if(PrefabsPool.GetInstance()._UIPrefabBindingDic[LastUITAG].NeedGoBack)
						{
							InstantiateGoBack(Gameobj);
						}

						NowUITAG = LastUITAG;
						LastUITAG = _LastUITAGDic[LastUITAG];

						Debug.Log(LastUITAG + "    加载成功！");
					}
				}
				else
				{
					Debug.LogError(LastUITAG);
				}
			}
			else
			{
				Debug.LogError(LastUITAG);
			}
		}
	}
	/// <summary>
	/// 实例化返回按钮
	/// </summary>
	private void InstantiateGoBack(GameObject GoBackParent)
	{
		Object GoBackobj = Resources.Load ("Prefabs/GoBack");
		GameObject GameGoBackobj = GameObject.Instantiate (GoBackobj) as GameObject;
		GameGoBackobj.name = GoBackobj.name;
		GameGoBackobj.transform.parent = GoBackParent.transform;
		GameGoBackobj.transform.localScale = Vector3.one;
	}
	
}



