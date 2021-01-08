import React, { Fragment } from 'react';
import { Pagination, Table } from 'semantic-ui-react';
import { User } from '../../app/api/agent';

const ListUsers = () => {
  const [users, setUsers] = React.useState([]);
  const [pagination, setPagination] = React.useState(0);

  React.useEffect(() => {
    User.getUsers(3, 1).then((result) => {
      setUsers(result.data);
      setPagination(JSON.parse(result.headers['x-pagination']));
    });
  }, []);

  const pageChangeHandler = async (e, { activePage }) => {
    const result = await User.getUsers(3, activePage);
    setUsers(result.data);
    setPagination(JSON.parse(result.headers['x-pagination']));
  };

  return (
    <Fragment>
      <h2>Employees</h2>

      <Table selectable celled>
        <Table.Header>
          <Table.Row>
            <Table.HeaderCell>Name</Table.HeaderCell>
            <Table.HeaderCell>Username</Table.HeaderCell>
            <Table.HeaderCell>Email</Table.HeaderCell>
          </Table.Row>
        </Table.Header>

        <Table.Body>
          {users &&
            users.map((user) => (
              <Table.Row key={user.username}>
                <Table.Cell>{user.displayName}</Table.Cell>
                <Table.Cell>{user.username}</Table.Cell>
                <Table.Cell>{user.email}</Table.Cell>
              </Table.Row>
            ))}
        </Table.Body>
      </Table>
      {pagination !== 0 && (
        <Pagination
          boundaryRange={0}
          activePage={pagination.CurrentPage}
          onPageChange={pageChangeHandler}
          siblingRange={1}
          totalPages={Math.ceil(pagination.TotalCount / pagination.PageSize)}
        />
      )}
    </Fragment>
  );
};

export default ListUsers;
