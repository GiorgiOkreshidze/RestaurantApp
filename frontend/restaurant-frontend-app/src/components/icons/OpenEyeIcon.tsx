interface Props {
  className?: string;
}

export const OpenEyeIcon: React.FC<Props> = ({ className }) => {
  return (
    <svg
      width="24"
      height="24"
      viewBox="0 0 24 24"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
      className={className}
    >
      <path
        d="M2.06153 12.3484C1.97819 12.1238 1.97819 11.8769 2.06153 11.6524C2.87323 9.68421 4.25104 8.0014 6.0203 6.81726C7.78955 5.63312 9.87057 5.00098 11.9995 5.00098C14.1285 5.00098 16.2095 5.63312 17.9788 6.81726C19.748 8.0014 21.1258 9.68421 21.9375 11.6524C22.0209 11.8769 22.0209 12.1238 21.9375 12.3484C21.1258 14.3165 19.748 15.9993 17.9788 17.1835C16.2095 18.3676 14.1285 18.9997 11.9995 18.9997C9.87057 18.9997 7.78955 18.3676 6.0203 17.1835C4.25104 15.9993 2.87323 14.3165 2.06153 12.3484Z"
        stroke="#232323"
        strokeWidth="1.75"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M12 15C13.6569 15 15 13.6569 15 12C15 10.3431 13.6569 9 12 9C10.3431 9 9 10.3431 9 12C9 13.6569 10.3431 15 12 15Z"
        stroke="#232323"
        strokeWidth="1.75"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  );
};
