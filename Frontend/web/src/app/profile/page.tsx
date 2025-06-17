"use client";

import React, { useEffect, useState } from "react";
import { getProfile } from "@/lib/api";
import { useAutoRefreshFetch } from "@/hooks/useAutoRefreshFetch";

const ProfilePage: React.FC = () => {
  const { handleRefreshToken } = useAutoRefreshFetch();
  const [profile, setProfile] = useState<{
    username: string;
    email: string;
    role: string;
  } | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const res = await handleRefreshToken();
        console.log("Profile fetch response:", res);
        // if (!res.ok) window.location.href = "/login";
        const result = await getProfile();
        setProfile(result);
      } catch {
        setError("ไม่สามารถดึงข้อมูลโปรไฟล์ได้");
      }
    };
    // ป้องกัน loop ไม่รู้จบจาก handleRefreshToken ที่เปลี่ยน reference ทุก render
    // ให้ใช้ useCallback ใน useAutoRefreshFetch หรือ disable exhaustive-deps
    fetchProfile();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [handleRefreshToken]);

  if (error) {
    return <div>{error}</div>;
  }

  if (!profile) {
    return <div>Loading...</div>;
  }

  return (
    <div>
      <h1>โปรไฟล์</h1>
      <p>ชื่อผู้ใช้: {profile.username}</p>
      <p>อีเมล: {profile.email}</p>
      <p>บทบาท: {profile.role}</p>
    </div>
  );
};

export default ProfilePage;
