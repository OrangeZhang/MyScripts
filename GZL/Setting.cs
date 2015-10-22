using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Setting
{
	/// <summary>
	/// 单例
	/// </summary>
	private static Setting Instance = null;
	/// <summary>
	/// 配置数据字典
	/// </summary>
	private Dictionary<string, string> SettingDic = new Dictionary<string, string> ();

	public static Setting GetInstance() {
		if (null == Instance) {
			Instance = new Setting();
		}
		return Instance;
	}

	/// <summary>
	/// 文件加载 
	/// </summary>
	private Setting(){

	}




	/// <summary>
	/// 例子
	/// </summary>
	public string Time(string key) {
		if (SettingDic.ContainsKey (key)) {
			return SettingDic [key];
		} else {
			return "";
		}
	}
}
