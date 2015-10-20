using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.IO;

public class FileUtils{
	private string _WriteablePath = "";

	//单例
	private static FileUtils instance = null;
	public static FileUtils getInstance(){
		if (null == instance) {
			instance = new FileUtils();
		}
		return instance;
	}
	private FileUtils(){
		_WriteablePath = Application.persistentDataPath;
	}

	//回到目录
	public string GetWriteablePath(){
		return _WriteablePath;
	}

	//创建文件夹
	public void CreateDirectory(string strPath){
		string strTargetPath = _WriteablePath + @"/" + strPath; //@在c#中为强制不转义的符号,在里面的转义字符无效
		if (!Directory.Exists (strTargetPath)) {
			Directory.CreateDirectory (strTargetPath);
		} else {
			Debug.LogError("Directory already exist");
			return;
		}
	}

	//检查目录是否存在
	public bool DirectoryExists(string strPath){
		string strTargetPath = _WriteablePath + @"/" + strPath;
		return Directory.Exists(strTargetPath);
	}

	//删除目录
	public void DeleteDirectory(string strPath){
		string strTargetPath = _WriteablePath + @"/" + strPath;
		if (Directory.Exists (strTargetPath)) {
			Directory.Delete (strTargetPath, true);
		} else {
			Debug.LogError("Directory does not exist");
			return;
		}
	}


	
	//检测文件是否存在
	public bool FileExists(string strFilePath){
		string strTargetPath = _WriteablePath + @"/" + strFilePath;
		return File.Exists(strTargetPath);
	}

	/// <summary>
	/// 以文本形式写入数据
	/// </summary>
	/// <param name="name">文件名（带文件类型后缀）.</param>
	/// <param name="info">写入内容.</param>
	/// <param name="overwrite">是否覆盖</param>
	public void WriteFileWithText(string name, string info, bool overwrite = true){
		string strTargetPath = _WriteablePath + @"/" + name;
		//文件流信息
		StreamWriter sw = null;
		FileInfo fi = new FileInfo (strTargetPath);
		if (!fi.Exists) {
			sw = fi.CreateText();
		} else {
			if(overwrite){
				sw = fi.CreateText();//覆盖写入
			}else{
				sw = fi.AppendText();//添加写入
			}
		}
		//以行的形式写入信息
		sw.WriteLine (info);
		//关闭流
		sw.Close();
		//销毁流
		sw.Dispose();
	}

	/// <summary>
	/// 读取流文件
	/// </summary>
	/// <param name="name">文件名（带文件类型后缀）.</param>
	public string ReadFileWithText(string name){
		string data = "";
		string strTargetPath = _WriteablePath + @"/" + name;

		if (File.Exists (strTargetPath)) {
			FileInfo fi = new FileInfo (strTargetPath);
			StreamReader sr = null;
			sr = fi.OpenText ();
			data = sr.ReadToEnd ();
			sr.Close ();
			sr.Dispose ();
		} else {
			Debug.LogError("File does not exist");
			return null;
		}
		return data;
	}

	/// <summary>
	/// 写为二进制文件
	/// </summary>
	/// <param name="name">文件名（带文件类型后缀）.</param>
	/// <param name="buf">buf.</param>
	public void WriteFileWithBinary(string name, byte[] buf){
		string strTargetPath = _WriteablePath + @"/" + name;
		FileStream fs = null;
		BinaryWriter bw = null;

		try{
			FileInfo t = new FileInfo(strTargetPath);
			fs = t.Create();
			bw = new BinaryWriter(fs,Encoding.UTF8);
			//二进制写入
			bw.Write(buf);
			//关闭二进制写入
			bw.Close();
			//销毁流
			fs.Dispose();
		}catch(IOException _e){
			Debug.LogError(_e.ToString());
		}
	}

	/// <summary>
	/// 二进制读取流文件
	/// </summary>
	/// <returns>The file with binary.</returns>
	/// <param name="name">文件名（带文件类型后缀）.</param>
	public byte[] ReadFileWithBinary(string name){
		string strTargetPath = _WriteablePath + @"/" + name;
		FileStream fs = null;
		BinaryReader br = null;

		if (File.Exists (strTargetPath)) {
			FileInfo t = new FileInfo (strTargetPath);
			fs = t.OpenRead ();
			if (null == fs) {
				return null;
			}
			br = new BinaryReader (fs, Encoding.UTF8);
			byte[] buf = br.ReadBytes ((int)fs.Length);
			br.Close ();
			fs.Dispose ();
			if (null != buf && buf.Length > 0) {
				return buf;
			}
		} else {
			Debug.LogError("File does not exist");
		}
		return null;
	}

	//删除文件
	public void DeleteFile(string name){
		string strTargetPath = _WriteablePath + @"/" + name;
		if(File.Exists(strTargetPath)){
			File.Delete(strTargetPath);
		}else{
			Debug.LogError("File does not exist");
			return;
		}
	}

	/// <summary>
	/// 拷贝文件或者文件夹  请填写完整位置
	/// </summary>
	/// <param name="startPath">起始文件位置</param>
	/// <param name="targetPath">目标位置</param>
	public bool CopyFileOrDirectory(string startPath, string destPath, bool isReplace = false){
		try{
			//创建目的文件夹
			if(!Directory.Exists(startPath)){
				Debug.LogError("StartPath File does not exist");
				return false;
			}

			if(!Directory.Exists(destPath)){
				Directory.CreateDirectory (destPath);
			}else{
				if(!isReplace){
					Debug.LogError("DestPath File already exist");
					return false;
				}
			}

			//循环子文件夹
			DirectoryInfo sDir = new DirectoryInfo(startPath);
			DirectoryInfo dDir = new DirectoryInfo(destPath);
			DirectoryInfo[] subDirArray = sDir.GetDirectories();
			foreach(DirectoryInfo subDir in subDirArray){
				CopyFileOrDirectory(subDir.FullName,destPath + @"/" + subDir.Name);
			}

			//拷贝文件
			FileInfo[] fileArray = sDir.GetFiles();
			foreach(FileInfo file in fileArray){
				file.CopyTo(destPath + @"/" + file.Name,true);
			}
		}catch(Exception _e)
		{
			Debug.LogError(_e.ToString());
			return false;
		}
		return true;
	}

	/// <summary>
	/// 文件移动 请填写完整位置
	/// </summary>
	/// <returns><c>true</c>, if file or directory was moved, <c>false</c> otherwise.</returns>
	/// <param name="startPath">起始文件位置.</param>
	/// <param name="destPath">目标位置.</param>
	public bool MoveFileOrDirectory(string startPath, string destPath, bool isReplace = false){
		if (!Directory.Exists (startPath)) {
			Debug.LogError("StartPath File does not exist");
			return false;
		}
		if (Directory.Exists (destPath)) {
			if(isReplace){
				Directory.Move (startPath, destPath);
			}else{
				Debug.LogError("DestPath File already exist");
				return false;
			}
		}
		return true;
	}
}
