using UnityEngine;
using System.Collections;

public class OnGoBackClick : MonoBehaviour 
{
	void OnClick()
	{
		UIControler.GetInstance ().GoBack ();
	}
}
