using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

namespace Olimpiadas
{
    public class PlayerInput : CharacterInput
    {
        private StarterAssetsInputs _input;

        // Start is called before the first frame update
        void Awake()
        {
            _input = GetComponent<StarterAssetsInputs>();
        }

        // Update is called once per frame
        void Update()
        {
            look = _input.look;
            move = _input.move;
        }
    }
}