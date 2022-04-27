using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Test : MonoBehaviour
{
    public Element element = Element.Holy;

    void Start()
    {
        Debug.Log(Enum.GetName(typeof(Element), element));
    }

}
