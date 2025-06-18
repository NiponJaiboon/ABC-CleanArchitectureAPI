"use client";

import React, { useEffect, useState } from "react";
import { getProfile } from "@/lib/api";
import { useFetch } from "@/hooks/useFetch";

const ProfilePage: React.FC = () => {
  const { handleRefreshToken } = useFetch();
  const [profile, setProfile] = useState<{
    username: string;
    email: string;
    role: string;
  } | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        await handleRefreshToken();
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
    <div className="max-w-md mx-auto mt-10 p-8 bg-white rounded-xl shadow-2xl border-2 border-blue-500">
      <h1 className="text-4xl font-extrabold text-blue-700 mb-6 text-center drop-shadow-lg">
        โปรไฟล์
      </h1>
      <div className="space-y-4 text-lg">
        <p>
          <span className="font-semibold text-gray-700">ชื่อผู้ใช้:</span>
          <span className="ml-2 text-blue-900 text-xl font-bold">
            {profile.username}
          </span>
        </p>
        <p>
          <span className="font-semibold text-gray-700">อีเมล:</span>
          <span className="ml-2 text-blue-900">{profile.email}</span>
        </p>
        <p>
          <span className="font-semibold text-gray-700">บทบาท:</span>
          <span className="ml-2 text-green-700 font-bold uppercase">
            {profile.role}
          </span>
        </p>
      </div>
    </div>
  );
};

export default ProfilePage;
