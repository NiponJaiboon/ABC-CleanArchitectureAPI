web/
├── public/
├── src/ # Source Code  
│ ├── app/ # Next.js App Router (Pages and Layouts)  
│ │ ├── login/
│ │ │ ├── page.tsx # Login Page  
│ │ │ └── ...
│ │ ├── profile/  
│ │ │ ├── page.tsx # Profile Page  
│ │ │ └── ...
│ │ ├── dashboard/  
│ │ │ ├── page.tsx # Dashboard Page
│ │ │ └── ...
│ │ ├── cms/  
│ │ │ ├── page.tsx # CMS Page
│ │ │ └── ...
│ │ ├── layout.tsx # Root Layout
│ │ └── page.tsx # Home Page  
│ ├── components/ # Reusable Components  
│ │ ├── common/ # Common UI Components
│ │ │ ├── Button.tsx
│ │ │ ├── Input.tsx
│ │ │ └── ...
│ │ ├── auth/ # Authentication Components
│ │ │ ├── LoginForm.tsx
│ │ │ └── ...
│ │ ├── ui/ # Specific UI Components
│ │ │ ├── Navbar.tsx
│ │ │ ├── Footer.tsx
│ │ │ ├── ProfileCard.tsx
│ │ │ └── CMSForm.tsx
│ │ └── ...
│ ├── hooks/ # Custom React Hooks
│ │ ├── useAuth.ts
│ │ ├── useFetch.ts
│ │ └── ...
│ ├── lib/ # Utility Functions and API Clients
│ │ ├── api.ts # API Client (Axios/Fetch)
│ │ ├── auth.ts # Authentication Logic
│ │ └── ...
│ ├── redux/ # Redux Store and Actions
│ │ ├── store.ts # Redux Store Configuration
│ │ ├── reducers/ # Reducers
│ │ │ ├── authReducer.ts
│ │ │ └── ...
│ │ ├── actions/ # Actions
│ │ │ ├── authActions.ts
│ │ │ └── ...
│ │ └── ...
│ ├── styles/ # Global Styles and CSS Modules
│ │ ├── globals.css
│ │ └── ...
│ ├── types/ # TypeScript Types
│ │ ├── user.ts # User Type Definition
│ │ └── ...
│ ├── utils/ # Utility Functions
│ │ ├── date.ts # Date Formatting Functions
│ │ └── ...
│ ├── data/ # Mock Data (for development)
│ │ ├── projects.json
│ │ ├── users.json
│ │ └── ...
│ └── ...
├── README.md # Project Documentation  
├── package.json # Project Dependencies and Scripts  
├── tsconfig.json # TypeScript Configuration
├── next.config.js # Next.js Configuration  
└── .env.local # Environment Variables
