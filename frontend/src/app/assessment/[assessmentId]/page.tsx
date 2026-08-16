import { AssessmentWizard } from "@/components/assessment/AssessmentWizard";

interface AssessmentPageProps {
  params: Promise<{
    assessmentId: string;
  }>;
}

export default async function AssessmentPage({ params }: AssessmentPageProps) {
  const { assessmentId } = await params;

  return (
    <main className="min-h-[calc(100vh-7.5rem)] bg-slate-100">
      <div className="mx-auto w-full max-w-6xl px-4 py-7 sm:px-8 sm:py-10 lg:px-10 lg:py-12">
        <AssessmentWizard assessmentId={assessmentId} />
      </div>
    </main>
  );
}
