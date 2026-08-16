import type { Metadata } from "next";
import type { ReactNode } from "react";
import { CandidateHeader } from "@/components/shell/CandidateHeader";
import { DisclaimerBar } from "@/components/shell/DisclaimerBar";
import "./globals.css";

export const metadata: Metadata = {
  title: "SFÉRA Candidate Engineering Demo",
  description:
    "Unofficial candidate engineering demo: explainable solution discovery, deterministic rules and structured sales handoff.",
  robots: {
    index: false,
    follow: false,
  },
};

export default function RootLayout({ children }: Readonly<{ children: ReactNode }>) {
  return (
    <html lang="sk">
      <body>
        <CandidateHeader />
        <DisclaimerBar />
        {children}
      </body>
    </html>
  );
}
