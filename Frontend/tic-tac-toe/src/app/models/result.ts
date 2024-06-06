export type Result<T = null> = {
  value: T;
  isSuccess: boolean;
  message?: string;
};
