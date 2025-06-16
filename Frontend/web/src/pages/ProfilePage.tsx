const API_URL = process.env.NEXT_PUBLIC_API_URL;
import React, { useEffect, useState } from "react";
import { useAutoRefreshFetch } from "@/hooks/useAutoRefreshFetch";

// ...นำฟังก์ชัน fetchWithAutoRefresh และ refreshToken มาไว้ที่เดียวกัน...

function ProfilePage() {
  const fetchWithAutoRefresh = useAutoRefreshFetch();
  const [profile, setProfile] = useState<unknown>(null);

  useEffect(() => {
    fetchWithAutoRefresh(`${API_URL}/api/Profile`)
      .then((res) => res.json())
      .then((data) => setProfile(data))
      .catch((err) => {
        // handle error เช่น redirect ไป login
        if (err.message === "Session expired") {
          window.location.href = "/login"; // หรือใช้ router.push ใน Next.js
        }
        console.error(err);
      });
  }, [fetchWithAutoRefresh]);

  return (
    <div>
      <h1>Profile</h1>
      <pre>{JSON.stringify(profile, null, 2)}</pre>
    </div>
  );
}

export default ProfilePage;
