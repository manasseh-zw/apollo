export default function BentoGrid() {
  return (
    <div className="mt-10 grid gap-2 lg:grid-cols-3 lg:grid-rows-2">
      {/* Data Collection Card */}
      <div className="relative lg:row-span-2">
        <div className="absolute inset-px rounded-lg bg-[#CC785C] lg:rounded-l-xl"></div>
        <div className="relative flex h-full flex-col overflow-hidden rounded-xl lg:rounded-l-xl">
          <div className="px-8 pb-3 pt-8 sm:px-10 sm:pb-0 sm:pt-10">
            <p className="text-lg font-medium tracking-tight text-gray-100 text-center">
              Think Different
            </p>
            <p className="mt-2 text-sm/6 text-secondary-50 text-center">
              Begin collecting research data by importing files or connecting to
              external sources
            </p>
          </div>
          <div className="flex flex-1 items-center justify-center p-6">
            <div className="flex flex-col items-center justify-center ">
              <img src="/doodle/deepthink.webp" className="size-80" alt="" />
            </div>
          </div>
        </div>
        <div className="pointer-events-none absolute inset-px rounded-lg shadow ring-1 ring-black/5 lg:rounded-l-xl"></div>
      </div>

      {/* Analysis Tools Card */}
      <div className="relative max-lg:row-start-1">
        <div className="absolute inset-px rounded-lg bg-[#F0E6D0] max-lg:rounded-t-[2rem]"></div>
        <div className="relative flex h-full flex-col overflow-hidden rounded-[calc(theme(borderRadius.lg)+1px)] max-lg:rounded-t-[calc(2rem+1px)]">
          <div className="px-8 pt-8 sm:px-10 sm:pt-10">
            <p className="text-lg font-medium tracking-tight text-gray-950 text-center">
              I do what is required
            </p>
          </div>
          <div className="flex flex-1 items-center justify-center ">
            <div className="flex flex-col items-center justify-center text-indigo-600">
              <img src="/doodle/quill.webp" className="size-48 mb-3" alt="" />
            </div>
          </div>
        </div>
        <div className="pointer-events-none absolute inset-px rounded-lg shadow ring-1 ring-black/5 max-lg:rounded-t-[2rem]"></div>
      </div>

      {/* Collaboration Card */}
      <div className="relative max-lg:row-start-3 lg:col-start-2 lg:row-start-2">
        <div className="absolute inset-px rounded-lg bg-[#BCD1CA]"></div>
        <div className="relative flex h-full flex-col overflow-hidden rounded-[calc(theme(borderRadius.lg)+1px)]">
          <div className="px-8 pt-8 sm:px-10 sm:pt-10">
            <p className="text-lg font-medium tracking-tight text-primary-700 text-center">
              Don't optmize what doesn't exist
            </p>
          </div>
          <div className="flex flex-1 items-center justify-center p-6">
            <div className="flex flex-col items-center justify-center text  -indigo-600">
              <img src="/doodle/think.webp" className="size-40 mb-3" alt="" />
            </div>
          </div>
        </div>
        <div className="pointer-events-none absolute inset-px rounded-lg shadow ring-1 ring-black/5"></div>
      </div>

      {/* Publication Card */}
      <div className="relative lg:row-span-2">
        <div className="absolute inset-px rounded-lg bg-[#788D5D] max-lg:rounded-b-xl lg:rounded-r-xl"></div>
        <div className="relative flex h-full flex-col overflow-hidden rounded-xl max-lg:rounded-b-xl lg:rounded-r-xl">
          <div className="px-8 pb-3 pt-8 sm:px-10 sm:pb-0 sm:pt-10">
            <p className="text-lg font-medium tracking-tight text-gray-100 text-center">
              What did you do today?
            </p>
            <p className="mt-2 text-sm/6 text-secondary-50 text-center">
              Format and export your research for publication or presentation
            </p>
          </div>
          <div className="flex flex-1 items-center justify-center p-6">
            <div className="flex flex-col items-center justify-center ">
              <img src="/doodle/handheart.webp" className="size-80" alt="" />
            </div>
          </div>
        </div>
        <div className="pointer-events-none absolute inset-px rounded-lg shadow ring-1 ring-black/5 max-lg:rounded-b-xl lg:rounded-r-xl"></div>
      </div>
    </div>
  );
}
