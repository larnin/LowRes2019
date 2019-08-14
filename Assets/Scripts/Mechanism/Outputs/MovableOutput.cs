using UnityEngine;
using System.Collections;

enum MoveType
{
    MovingWhenActive,
    SwitchSideOnActivation,
    ProgressWhenActive,
}

public class MovableOutput : BaseOutput
{
    [SerializeField] Path m_path = null;
    [SerializeField] bool m_reverseDirection = false;
    [SerializeField] float m_movingSpeed = 1;
    [SerializeField] MoveType m_moveType = MoveType.MovingWhenActive;

    float m_pathLenght = 0;
    bool m_active = false;
    bool m_PositiveMove = false;

    float m_currentPathPos = 0;

    private void Start()
    {
        if(m_path == null)
        {
            Debug.LogError("No path set on this MovableOutput");
            enabled = false;
            return;
        }

        m_pathLenght = m_path.GetLenght();
    }

    public override void SetActiveStatus(bool active)
    {
        m_active = active;
        if (m_active && m_moveType == MoveType.SwitchSideOnActivation)
            m_PositiveMove = !m_PositiveMove;
    }

    private void Update()
    {
        float nextPathPos = m_currentPathPos;
        float deltaPos = m_movingSpeed * Time.deltaTime;

        switch(m_moveType)
        {
        case MoveType.MovingWhenActive:
            if(m_active)
            {
                if (m_PositiveMove)
                    nextPathPos += deltaPos;
                else nextPathPos -= deltaPos;
                if (nextPathPos < 0)
                {
                    nextPathPos = 0;
                    m_PositiveMove = !m_PositiveMove;
                }
                else if(nextPathPos > m_pathLenght)
                {
                    nextPathPos = m_pathLenght;
                    m_PositiveMove = !m_PositiveMove;
                }
            }
            break;
        case MoveType.ProgressWhenActive:
            if(m_active)
                nextPathPos += deltaPos;
            else nextPathPos -= deltaPos;
            if (nextPathPos < 0)
                nextPathPos = 0;
            else if (nextPathPos > m_pathLenght)
                nextPathPos = m_pathLenght;
            break;
        case MoveType.SwitchSideOnActivation:
            if (m_active)
            {
                if (m_PositiveMove)
                    nextPathPos += deltaPos;
                else nextPathPos -= deltaPos;
                if (nextPathPos < 0)
                    nextPathPos = 0;
                else if (nextPathPos > m_pathLenght)
                    nextPathPos = m_pathLenght;
            }
            break;
        }

        m_currentPathPos = nextPathPos;

        if (m_reverseDirection)
            nextPathPos = m_pathLenght - nextPathPos;

        var pos = m_path.GetPosAt(nextPathPos);

        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }
}
