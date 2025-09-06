# Endpoints for User 
### Get current user Profile
GET: `/api/User/profile`

**Response**

Status Code: 200

```json
{
  "name": "string",
  "email": "string",
  "lastSeen": "2025-09-06T23:04:42.634Z",
  "bio": "string",
  "avatarPath": "string",
  "followerCount": 0
}
```

---------
### Get User By 
GET: `/api/User/profile/{userId}`
Description: Get user by id

**Response**

Status Code: 200

```json
{
  "name": "string",
  "email": "string",
  "lastSeen": "2025-09-06T23:04:42.634Z",
  "bio": "string",
  "avatarPath": "string",
  "followerCount": 0
}
```

---------
### Delete current user
Delete: `/api/User/profile/{userId}`

**Response**
Status Code: 200

---------
### Update User
POST: `/api/User/profile/update/{userId}`

**Request**
```json
{
  "name": "string",
  "bio": "string",
  "avatarPath": "string"
}
```

**Response**

Status Code: 200

```json
{
  "name": "string",
  "email": "string",
  "lastSeen": "2025-09-06T23:10:21.411Z",
  "bio": "string",
  "avatarPath": "string",
  "followerCount": 0
}
```

---------
### Follow
POST: `/api/User/profile/{followerId}/follow/{targetUserId}`

**Response**

Status Code: 200

---------
### Unfollow
POST: `/api/User/profile/{followerId}/unfollow/{targetUserId}`

**Response**

Status Code: 200

---------
### Update Password
GET: `/api/User/profile/update/pass/{userId}`

**Request**
```json
{
  "currentPassword": "string",
  "updatedPassword": "string"
}
```

**Response**

Status Code: 200
