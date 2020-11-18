using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZenTarget
{
    public class Target : MonoBehaviour
    {

        public Target()
        {

        }

        public List<GameObject> Cibles;
        Random rnd = new Random();

        // Start is called before the first frame update

        public void ClearTarget()
        {

            foreach (var item in Cibles)
            {
                item.SetActive(false);
            }

        }


        public void NewTarget()
        {
            Cibles[Random.Range(0, Cibles.Count)].SetActive(true);
        }

        void Start()
        {
            NewTarget();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}