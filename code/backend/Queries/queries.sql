-- name: GetCitizen :one
SELECT *
  FROM citizen
 WHERE uruguayan_id = ?;
