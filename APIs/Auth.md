# Endpoints for Authentication

### Register 
POST: `/api/Auth/register`

**Request**
```json
{
  "name": "string",
  "username": "string",
  "email": "string",
  "bio": "string",
  "avatarPath": "string",
  "passwrod": "string",
  "lastSeen": "2025-09-06T22:27:00.026Z"
}
```
**Response**

Status Code: 200

```json
{
  "success": true,
  "errors": [
    "string"
  ],
  "data": {
    "isAuthenticated": true,
    "token": "string",
    "refreshToken": {
      "token": "string",
      "expireOn": "2025-09-06T22:57:05.109Z",
      "isExpired": true,
      "revokedOn": "2025-09-06T22:57:05.109Z",
      "isActive": true,
      "createdOn": "2025-09-06T22:57:05.109Z"
    },
    "user": {
      "name": "string",
      "email": "string",
      "lastSeen": "2025-09-06T22:57:05.109Z",
      "bio": "string",
      "avatarPath": "string",
      "followerCount": 0
    }
  }
}
```

-----
### Login 
POST: `/api/Auth/login`

**Request**
```json
{
  "email": "string",
  "password": "string"

}
```
**Response**

Status Code: 200

```json
{
  "success": true,
  "errors": [
    "string"
  ],
  "data": {
    "isAuthenticated": true,
    "token": "string",
    "refreshToken": {
      "token": "string",
      "expireOn": "2025-09-06T22:57:05.109Z",
      "isExpired": true,
      "revokedOn": "2025-09-06T22:57:05.109Z",
      "isActive": true,
      "createdOn": "2025-09-06T22:57:05.109Z"
    },
    "user": {
      "name": "string",
      "email": "string",
      "lastSeen": "2025-09-06T22:57:05.109Z",
      "bio": "string",
      "avatarPath": "string",
      "followerCount": 0
    }
  }
}
```

------
### Refresh Token
POST: `/api/Auth/refresh-token/{id}`
Description: refersh token when jwt token is expired

**Response**

Status Code: 200
```json
{
  "success": true,
  "errors": [
    "string"
  ],
  "data": {
    "isAuthenticated": true,
    "token": "string",
    "refreshToken": {
      "token": "string",
      "expireOn": "2025-09-06T22:57:05.109Z",
      "isExpired": true,
      "revokedOn": "2025-09-06T22:57:05.109Z",
      "isActive": true,
      "createdOn": "2025-09-06T22:57:05.109Z"
    },
    "user": {
      "name": "string",
      "email": "string",
      "lastSeen": "2025-09-06T22:57:05.109Z",
      "bio": "string",
      "avatarPath": "string",
      "followerCount": 0
    }
  }
}
```

---------
### Logout
POST: `/api/Auth/logout/{id}`
Description: refersh token when jwt token is expired

**Response**

Status Code: 200
```json
{
  "success": true,
  "errors": [
    "string"
  ],
  "data": true
}
```
