using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ObjectPool: MonoBehaviour {

	//Public的组件属性
	[System.Serializable]
	public struct PrefabToPool{
		public GameObject prefab;
		public int maximum;
	}
	public PrefabToPool[] prefabsToPool;

	//单例实例
	public static ObjectPool instance = null;

	//主对象池字典
	private Dictionary<string, List<GameObject>> pools;

	//收集所有的预制体   耗尽时创建更多的预制体
	private Dictionary<string, GameObject> prefabs;

	// --------------------------------------------------------------------- 事件处理程序
	private void Awake(){
		if (instance == null) {
			instance = this;
			//设置这个在场景重新加载时不被删除
			DontDestroyOnLoad(gameObject);
			//构建字典
			pools = new Dictionary<string, List<GameObject>>();
			prefabs = new Dictionary<string, GameObject>();

			//构建对象池
			for (int n = 0; n < prefabsToPool.Length; n++) {
				//构建新的池对象
				List<GameObject> newPool = new List<GameObject>();

				//从公有的组件数据获取属性
				int max = prefabsToPool[n].maximum;
				GameObject prefab = prefabsToPool[n].prefab;
				//在字典中存储原始预制体
				prefabs.Add(prefab.name,prefab);

				//实例所有的游戏对象并其添加到新的池中
				for(int i = 0; i < max; i++){
					GameObject obj = Instantiate(prefab) as GameObject;
					//设置它以便在场景改变（对象池失败）的时候不删除
					DontDestroyOnLoad(obj);
					obj.transform.parent = this.transform;
					obj.SetActive(false);
					newPool.Add(obj);
				}

				//添加新的池（列表）到字典与关键是组合式的名字
				pools.Add(prefab.name,newPool);
			}
		}else if(instance != this){
			//然后摧毁 这执行我们的单例模式,也就是说只能有一个实例GameManager
			Destroy(gameObject);
		}
	}

	// -------------------------------------------------------------------- 公有方法
	//通过对象池实例化游戏对象并且返回它
	public GameObject Get(string prefabsName){
		//从字典总获取池
		List<GameObject> pool = pools [prefabsName];

		//找到没有激活的游戏对象并且返回他
		foreach (GameObject obj in pool) {
			if(!obj.activeInHierarchy){
				obj.SetActive(true);
				return obj;
			}
		}

		//我们已经耗尽池中的游戏对象,危险,危险!
		GameObject prefab = prefabs [prefabsName];
		GameObject newObj = Instantiate (prefab) as GameObject;

		//设置它以便在场景改变（对象池失败）的时候不删除
		DontDestroyOnLoad (newObj);
		newObj.transform.parent = this.transform;
		newObj.SetActive (false);
		pool.Add (newObj);
		return Get (prefabsName);
	}

	//通过对象池实例化游戏对象和位置/旋转(像Instantiate())并且返回它
	public GameObject Get(string prefabName, Vector3 position, Quaternion rotation){
		GameObject obj = Get (prefabName);
		//位置与旋转
		obj.transform.position = position;
		obj.transform.rotation = rotation;
		return obj;
	}

	//从对象池实例化GameObject但返回一个基于名称的组件
	public Component GetComponent(string prefabName, string scriptName){
		GameObject obj = Get (prefabName);
		return obj.GetComponent(scriptName);
	}

	//返回游戏对象到对象池
	public void Dispose(GameObject obj){
		obj.SetActive (false);
	}

	//处理所有给出名称的游戏对象
	public void Dispose(string prefabName){
		//从字典中取出池
		List<GameObject> pool = pools [prefabName];
		//找到没有机会的游戏对象并且返回它
		foreach (GameObject obj in pool) {
			Dispose(obj);
		}
	}
}
