export function IntFormatter(views) 
{ 
  return views > 1000 ? `${(views/1000).toFixed(1)}K` : views
}