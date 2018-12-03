using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour {

    public LayerMask collisionMask;

    const float skinWidth = .015f;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    float maxSlopeAngle = 50;

    float horizontalRaySpacing;
    float verticalRaySpacing;

    [HideInInspector]
    public BoxCollider2D entityCollider;
    protected RaycastOrigins raycastOrigins;

    [HideInInspector]
    public Vector2 input;

    public CollisionInfo collisions;

    void Awake() {
        entityCollider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    void Start() {
        CalculateRaySpacing();
    }

    public void Move(Vector2 velocity) {
        collisions.Reset();

        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.velocityOld = velocity;

        if (velocity.y < 0) {
            DescendSlope(ref velocity);
        }
        if (velocity.x != 0) {
            HorizontalCollision(ref velocity);
        }

        if (velocity.y != 0) {
            VerticalCollisions(ref velocity);
        }
    
        transform.Translate(velocity);

        CheckOnStairs();
        CheckUnderStairs();
    }

    void HorizontalCollision(ref Vector2 velovity) {
        float directionX = Mathf.Sign(velovity.x);
        float rayLength = Mathf.Abs(velovity.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++) {
            Vector2 rayOrigin = (directionX == -1 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight);
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit) {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxSlopeAngle) {
                    if (collisions.descendingSlope) {
                        collisions.descendingSlope = false;
                        velovity = collisions.velocityOld;
                    }
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.slopeAngleOld) {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        velovity.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref velovity, slopeAngle, hit.normal);
                    velovity.x += distanceToSlopeStart * directionX;
                }

                if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle) {
                    velovity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    if (collisions.climbingSlope) {
                        velovity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad * Mathf.Abs(velovity.x));
                    }

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }
    }

    void VerticalCollisions(ref Vector2 velocity) {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++) {
            Vector2 rayOrigin = (directionY == -1 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft);
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit) {
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if (collisions.climbingSlope) {
                    velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }

        if (collisions.climbingSlope) {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit) {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle) {
                    velocity.x = (hit.distance * skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                    collisions.slopeNormal = hit.normal;
                }
            }
        }
    }

    void ClimbSlope(ref Vector2 velocity, float slopeAngle, Vector2 slopeNormal) {
        float moveDst = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDst;

        if (velocity.y <= climbVelocityY) {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDst * Mathf.Sign(velocity.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
            collisions.slopeNormal = slopeNormal;
        }
        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDst * Mathf.Sign(velocity.x);
        collisions.below = true;
    }

    void DescendSlope(ref Vector2 velocity) {

        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(velocity.y) + skinWidth, collisionMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(velocity.y) + skinWidth, collisionMask);

        if (maxSlopeHitLeft ^ maxSlopeHitRight) {
            SlideDownMaxSlope(maxSlopeHitLeft, ref velocity);
            SlideDownMaxSlope(maxSlopeHitRight, ref velocity);
        }

        if (!collisions.slidingDownMaxSlope) {
            float directionX = Mathf.Sign(velocity.x);
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

            if (hit) {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle) {
                    if (Mathf.Sign(hit.normal.x) == directionX) {
                        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x)) {
                            float moveDst = Mathf.Abs(velocity.x);
                            float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDst;
                            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDst * Mathf.Sign(velocity.x);
                            velocity.y -= descendVelocityY;

                            collisions.slopeAngle = slopeAngle;
                            collisions.descendingSlope = true;
                            collisions.below = true;
                            collisions.slopeNormal = hit.normal;
                        }
                    }
                }
            }
        }
    }

    void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 velocity) {
        if (hit) {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle > maxSlopeAngle) {
                velocity.x = hit.normal.x * (Mathf.Abs(velocity.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

                collisions.slopeAngle = slopeAngle;
                collisions.slidingDownMaxSlope = true;
                collisions.slopeNormal = hit.normal;
            }
        }
    }

    void UpdateRaycastOrigins() {
        Bounds bounds = entityCollider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing() {
        Bounds bounds = entityCollider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 0, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    void CheckOnStairs() {
        Debug.DrawRay(raycastOrigins.bottomLeft, Vector2.down * 0.5f, Color.blue);
        Debug.DrawRay(raycastOrigins.bottomRight, Vector2.down * 0.5f, Color.blue);
        RaycastHit2D hitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, 0.5f, collisionMask);
        if (hitLeft.collider != null && hitLeft.transform.gameObject.layer == LayerMask.NameToLayer("Stairs")) {
            Debug.Log("on stairs LEft");
            collisions.onStairs = true;
        }
        else {
            RaycastHit2D hitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, 0.5f, collisionMask);
            if (hitRight.collider != null && hitRight.transform.gameObject.layer == LayerMask.NameToLayer("Stairs")) {
                collisions.onStairs = true;
                Debug.Log("on stairs RIght");
            }
        }
    }

    void CheckUnderStairs() {
        Debug.DrawRay(raycastOrigins.bottomLeft, Vector2.up * 2.5f, Color.cyan);
        Debug.DrawRay(raycastOrigins.bottomRight, Vector2.up * 2.5f, Color.cyan);
        RaycastHit2D hitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.up, 2.5f, LayerMask.GetMask("Stairs"));
        if (hitLeft.collider != null && hitLeft.transform.gameObject.layer == LayerMask.NameToLayer("Stairs")) {
            collisions.underStairs = true;
            return;
        }
        else {
            RaycastHit2D hitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.up, 2.5f, LayerMask.GetMask("Stairs"));
            if (hitRight.collider != null && hitRight.transform.gameObject.layer == LayerMask.NameToLayer("Stairs")) {
                collisions.underStairs = true;
                return;
            }
        }
        collisions.underStairs = false;
    }

    protected struct RaycastOrigins {
        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;
    }

    public struct CollisionInfo {
        public bool above;
        public bool below;
        public bool left;
        public bool right;

        public bool climbingSlope;
        public bool descendingSlope;
        public bool slidingDownMaxSlope;

        public float slopeAngle, slopeAngleOld;
        public Vector2 slopeNormal;
        public Vector3 velocityOld;

        public bool onStairs;
        public bool underStairs;

        public bool inCoverRange;
        public int availableCoverHeight;

        public void Reset() {
            above = below = left = right = false;
            climbingSlope = descendingSlope = false;
            slidingDownMaxSlope = false;

            slopeNormal = Vector2.zero;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;

            onStairs = false;
            underStairs = false;

            inCoverRange = false;
            availableCoverHeight = -1;
        }
    }
}
