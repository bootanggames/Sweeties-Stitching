using UnityEngine;

public interface INeedleResetPositions : IGameService
{
    Transform headNeedleResetPos {  get; }
    Transform eyeRightNeedleResetPos { get; }
    Transform earRightNeedleResetPos { get; }
    Transform earLeftNeedleResetPos { get; }
    Transform eyeLeftNeedleResetPos { get; }
    Transform armLeftNeedleResetPos { get; }
    Transform legLeftNeedleResetPos { get; }
    Transform legRightNeedleResetPos { get; }
    Transform armRightNeedleResetPos { get; }
}
