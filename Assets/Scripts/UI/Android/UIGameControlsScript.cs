using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class UIGameControlsScript : MonoBehaviour
{
    public void SetFiring(bool firing){
        Globals.Nozzle.SetFiring(firing);
    }

    public void SwitchFoam(){
        Globals.Nozzle.SwitchParticles();
    }

    public void Boost(){
        Globals.Player.Boost();
    }
}
