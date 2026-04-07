# API Reference

Full endpoint documentation for the AskFm Clone backend API. All endpoints return JSON responses.

---

## Auth (`/api/Auth`)

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `POST` | `/api/Auth/register` | ❌ | Register a new user |
| `POST` | `/api/Auth/login` | ❌ | Login and receive JWT + refresh token |
| `POST` | `/api/Auth/refresh-token/{id}` | ✅ | Refresh an expired access token |
| `POST` | `/api/Auth/logout/{id}` | ✅ | Revoke tokens and logout |
| `POST` | `/api/Auth/forgot-password` | ❌ | Send password reset email |
| `POST` | `/api/Auth/reset-password` | ❌ | Reset password with token |

---

## User (`/api/User`)

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `GET` | `/api/User/profile` | ✅ | Get current user's profile |
| `GET` | `/api/User/profile/{userId}` | ✅ | Get a specific user's profile |
| `POST` | `/api/User/profile/update/{userId}` | ✅ | Update user profile |
| `DELETE` | `/api/User/profile/{userId}` | ✅ | Soft-delete user account |
| `POST` | `/api/User/profile/{followerId}/follow/{targetUserId}` | ✅ | Follow a user |
| `POST` | `/api/User/profile/{followerId}/unfollow/{targetUserId}` | ✅ | Unfollow a user |
| `POST` | `/api/User/profile/update/pass/{userId}` | ✅ | Change password |

---

## Comment (`/api/Comment`)

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `GET` | `/api/Comment/{id}/likes` | ✅ | Get all likes for a comment |
| `POST` | `/api/Comment/{id}/likes` | ✅ | Like a comment |
| `DELETE` | `/api/Comment/{id}/likes` | ✅ | Remove like from a comment |

---

## Notification (`/api/Notification`)

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `GET` | `/api/Notification?pageNumber=1&pageSize=10` | ✅ | Get paginated notifications |
| `GET` | `/api/Notification/type/{category}` | ✅ | Get notifications by type |
| `PUT` | `/api/Notification/{notificationId}/read` | ✅ | Mark notification as read |
| `PUT` | `/api/Notification/read-all` | ✅ | Mark all notifications as read |


---

## Real-time (SignalR)

| Hub | Endpoint | Description |
|---|---|---|
| `NotificationHub` | `/notificationHub` | Real-time notification delivery (pass `access_token` as query param) |

### Connection

```javascript
// Connect with JWT token
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub", {
        accessTokenFactory: () => "your-jwt-token"
    })
    .build();
```

### Hub Methods

| Method | Direction | Description |
|---|---|---|
| `JoinUserGroup(userId)` | Client → Server | Subscribe to a user's notification group |
| `LeaveUserGroup(userId)` | Client → Server | Unsubscribe from a user's notification group |

> On connect, users are automatically added to their own group (`user_{userId}`) based on the JWT token. On disconnect, they are automatically removed.
