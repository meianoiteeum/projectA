using UnityEngine;

namespace Script.Core
{
    public class MapLine : MonoBehaviour
    {
        [Header("Colors por Status")]
        public Color colorReleased = Color.blue;
        public Color colorBlocked  = Color.gray;

        public ConnectionData Data { get; private set; }

        private LineRenderer _lr;

        public void Init(ConnectionData data, Vector3 fromPos, Vector3 toPos)
        {
            Data = data;
            _lr = GetComponent<LineRenderer>();
            if (_lr == null)
            {
                Debug.LogError($"LineRenderer não encontrado no prefab Line (conn {data.from}->{data.to}).");
                return;
            }

            _lr.positionCount = 2;
            _lr.SetPosition(0, fromPos);
            _lr.SetPosition(1, toPos);

            Color c = data.status == "L" ? colorReleased : colorBlocked;
            _lr.startColor = c;
            _lr.endColor   = c;
        }
    }
}
