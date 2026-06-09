export type TaskStatus = 'Open' | 'Done';

export interface TaskItem {
  id: string;
  title: string;
  description: string;
  createdDate: string;
  status: TaskStatus;
}
