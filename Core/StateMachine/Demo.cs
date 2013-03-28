//using System.Diagnostics;
//using UnityEngine;

//using System.Collections;



//public class Monster : MonoBehaviour
//{



//    public enum State
//    {

//        Crawl,

//        Walk,

//        Die,

//    }



//    public State state;



//    IEnumerator CrawlState()
//    {

//        Debug.Log("Crawl: Enter");

//        while (state == State.Crawl)
//        {

//            yield return 0;

//        }

//        Debug.Log("Crawl: Exit");

//        NextState();

//    }



//    IEnumerator WalkState()
//    {

//        Debug.Log("Walk: Enter");

//        while (state == State.Walk)
//        {

//            yield return 0;

//        }

//        Debug.Log("Walk: Exit");

//        NextState();

//    }



//    IEnumerator DieState()
//    {

//        Debug.Log("Die: Enter");

//        while (state == State.Die)
//        {

//            yield return 0;

//        }

//        Debug.Log("Die: Exit");

//    }



//    void Start()
//    {

//        NextState();

//    }



//    void NextState()
//    {

//        string methodName = state.ToString() + "State";

//        System.Reflection.MethodInfo info =

//            GetType().GetMethod(methodName,

//                                System.Reflection.BindingFlags.NonPublic |

//                                System.Reflection.BindingFlags.Instance);

//        StartCoroutine((IEnumerator)info.Invoke(this, null));

//    }



//}