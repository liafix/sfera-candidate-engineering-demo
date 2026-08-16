export function DisclaimerBar() {
  return (
    <div className="border-b border-amber-300/25 bg-amber-50 text-amber-950">
      <div className="mx-auto flex w-full max-w-7xl items-start gap-3 px-5 py-2.5 text-xs leading-5 sm:px-8 lg:px-10">
        <span aria-hidden="true" className="mt-1 size-1.5 shrink-0 rounded-full bg-amber-500" />
        <p>
          <strong>UNOFFICIAL CANDIDATE DEMO.</strong> Nie je to produkt ani interný systém SFÉRY. Používa verejný kontext,
          syntetické dáta a vysvetliteľné demo pravidlá; výsledky vyžadujú odbornú validáciu.
        </p>
      </div>
    </div>
  );
}
