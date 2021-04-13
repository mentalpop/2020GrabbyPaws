/******************************************************************************/
/*
  Project   - Boing Kit
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using UnityEngine;

namespace BoingKit
{
  // pre-render
  public class BoingManagerPostUpdatePump : MonoBehaviour
  {
    private void Start()
    {
      DontDestroyOnLoad(gameObject);
    }

    private bool TryDestroyDuplicate()
    {
      if (BoingManager.s_managerGo == gameObject)
        return false;

      // so reimporting scripts don't build up duplicate update pumps
      Destroy(gameObject);
      return true;
    }

    private void FixedUpdate()
    {
      if (TryDestroyDuplicate())
        return;

      BoingManager.FixedUpdate();
    }

    private void LateUpdate()
    {
      if (TryDestroyDuplicate())
        return;

      BoingManager.Execute();

      BoingManager.PullBehaviorResults();
      BoingManager.PullReactorResults();
      BoingManager.PullBonesResults(); // pull bones results last, so bone transforms don't inherit parent transform delta
    }
  }
}
