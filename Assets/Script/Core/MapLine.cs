using UnityEngine;

namespace Script.Core
{
    public class MapLine : MonoBehaviour
    {
        [Header("Colors por Status")]
        public Color colorReleased = Color.blue;
        public Color colorBlocked  = Color.gray;
        public Color colorVoidd    = new Color(0f, 0f, 0f, 0f);

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

            RefreshColor();
        }

        public void SetEndpoint(int index, Vector3 pos)
        {
            if (_lr == null) return;
            _lr.SetPosition(index, pos);
        }

        public Vector3 GetEndpoint(int index)
        {
            return _lr != null ? _lr.GetPosition(index) : Vector3.zero;
        }

        public void RefreshColor()
        {
            if (_lr == null) return;
            Color c = Data?.status switch
            {
                "L" => colorReleased,
                "V" => colorVoidd,
                _   => colorBlocked,
            };
            _lr.startColor = c;
            _lr.endColor   = c;
        }
    }
}
