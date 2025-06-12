This is a [Next.js](https://nextjs.org) project bootstrapped with [`create-next-app`](https://nextjs.org/docs/app/api-reference/cli/create-next-app).

## Getting Started

First, run the development server:

```bash
npm run dev
# or
yarn dev
# or
pnpm dev
# or
bun dev
```

Open [http://localhost:3000](http://localhost:3000) with your browser to see the result.

You can start editing the page by modifying `app/page.tsx`. The page auto-updates as you edit the file.

This project uses [`next/font`](https://nextjs.org/docs/app/building-your-application/optimizing/fonts) to automatically optimize and load [Geist](https://vercel.com/font), a new font family for Vercel.

## Learn More

To learn more about Next.js, take a look at the following resources:

- [Next.js Documentation](https://nextjs.org/docs) - learn about Next.js features and API.
- [Learn Next.js](https://nextjs.org/learn) - an interactive Next.js tutorial.

You can check out [the Next.js GitHub repository](https://github.com/vercel/next.js) - your feedback and contributions are welcome!

## Deploy on Vercel

The easiest way to deploy your Next.js app is to use the [Vercel Platform](https://vercel.com/new?utm_medium=default-template&filter=next.js&utm_source=create-next-app&utm_campaign=create-next-app-readme) from the creators of Next.js.

Check out our [Next.js deployment documentation](https://nextjs.org/docs/app/building-your-application/deploying) for more details.

web #โฟลเดอร์หลักของโปรเจกต์ 1
├── public/ #สำหรับไฟล์สาธารณะ เช่น รูปภาพ 1
│ └── images/ #เก็บไฟล์รูปภาพที่ใช้ในเว็บ 1
├── src/ #โค้ดหลักของแอปพลิเคชัน 1
│ ├── app/ #โฟลเดอร์สำหรับไฟล์เพจ (ตามโครงสร้าง Next.js 13+) 1
│ │ ├── login/page.tsx #หน้าเข้าสู่ระบบ 1
│ │ ├── profile/page.tsx #หน้าโปรไฟล์ผู้ใช้ 1
│ │ ├── dashboard/page.tsx #หน้าหลักหลังล็อกอิน 1
│ │ ├── cms/page.tsx #หน้าสำหรับจัดการเนื้อหา 1
│ │ ├── layout.tsx #โครงร่างหลักของแต่ละเพจ (เช่น Navbar, Footer) 1
│ │ └── page.tsx #หน้าแรกของเว็บไซต์ 1
│ ├── components/ #รวมคอมโพเนนต์ที่นำกลับมาใช้ซ้ำได้ 1
│ │ ├── Navbar.tsx #แถบนำทาง 1
│ │ ├── Footer.tsx #ส่วนท้าย 1
│ │ ├── LoginForm.tsx #ฟอร์มล็อกอิน 1
│ │ ├── ProfileCard.tsx #การ์ดแสดงข้อมูลโปรไฟล์ 1
│ │ └── CMSForm.tsx #ฟอร์มสำหรับจัดการเนื้อหา 1
│ ├── hooks/ #Custom React Hooks 1
│ │ ├── useAuth.ts #ฮุกสำหรับจัดการการยืนยันตัวตน 1
│ │ └── useFetch.ts #ฮุกสำหรับดึงข้อมูล 1
│ ├── styles/ #ไฟล์ CSS 1
│ │ └── globals.css #สไตล์หลักของโปรเจกต์ 1
│ ├── data/ #ข้อมูลจำลอง (mock data) 1
│ │ ├── projects.json #ข้อมูลโปรเจกต์ 1
│ │ └── users.json #ข้อมูลผู้ใช้ 1
│ └── lib/ #ฟังก์ชันหรือ utility ที่ใช้บ่อย 1
│   ├── auth.ts #ฟังก์ชันเกี่ยวกับการยืนยันตัวตน 1
│   └── api.ts #ฟังก์ชันสำหรับเรียก API 1
├── README.md #ไฟล์อธิบายโปรเจกต์ 1
├── package.json #รายละเอียด dependencies และสคริปต์ 1
├── tsconfig.json #การตั้งค่า TypeScript 1
├── next.config.js #การตั้งค่า Next.js 1
└── .env.local #ตัวแปรสภาพแวดล้อม (Environment Variables) เช่น คีย์ลับต่าง ๆ 1
